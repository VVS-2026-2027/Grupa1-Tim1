using BentoLab.Controllers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestiranje
{
    [TestClass]
    [DoNotParallelize]
    public class KameraControllerTests
    {
        private Mock<IWebHostEnvironment> _mockHostingEnvironment = null!;
        private KameraController _controller = null!;
        private string _tempWebRootPath = null!;
        private StringWriter _konzolaIzlaz = null!;
        private TextWriter _originalniIzlaz = null!;

        [TestInitialize]
        public void Setup()
        {
            // Preusmjeravamo izlaz konzole zbog poziva EmailService-a unutar SpasiSliku
            _originalniIzlaz = Console.Out;
            _konzolaIzlaz = new StringWriter();
            Console.SetOut(_konzolaIzlaz);

            // Kreiramo izolovani privremeni wwwroot folder za svaki test
            _tempWebRootPath = Path.Combine(Path.GetTempPath(), $"BentoLab_Test_{Guid.NewGuid():N}");
            Directory.CreateDirectory(_tempWebRootPath);

            _mockHostingEnvironment = new Mock<IWebHostEnvironment>();
            _mockHostingEnvironment.Setup(env => env.WebRootPath).Returns(_tempWebRootPath);

            _controller = new KameraController(_mockHostingEnvironment.Object);

            // Simuliramo TempData kako bi TempData["Poruka"] radio bez HttpContext sesije
            var httpContext = new DefaultHttpContext();
            _controller.TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>());
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.SetOut(_originalniIzlaz);
            _konzolaIzlaz.Dispose();
            _controller?.Dispose();

            // Oslobađamo resurse i brišemo privremeni folder nakon testa
            GC.Collect();
            GC.WaitForPendingFinalizers();
            if (Directory.Exists(_tempWebRootPath))
            {
                Directory.Delete(_tempWebRootPath, recursive: true);
            }
        }

        [TestMethod]
        public void Index_PozivAkcije_VracaViewResult()
        {
            // Act
            var rezultat = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(rezultat, typeof(ViewResult));
        }

        [TestMethod]
        [DataRow(1)]
        [DataRow(42)]
        public void UslikajBento_ProslijedjenId_PostavljaViewBagIVracaView(int narudzbaId)
        {
            // Act
            var rezultat = _controller.UslikajBento(narudzbaId) as ViewResult;

            // Assert
            Assert.IsNotNull(rezultat);
            Assert.AreEqual(narudzbaId, rezultat.ViewData["NarudzbaId"]);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("")]
        public async Task SpasiSliku_PrazanIliNullBase64_VracaBadRequest(string? neispravnaSlika)
        {
            // Act
            var rezultat = await _controller.SpasiSliku(neispravnaSlika!, 5);

            // Assert
            var badRequest = rezultat as BadRequestObjectResult;
            Assert.IsNotNull(badRequest);
            Assert.AreEqual("Slikanje nije uspjelo.", badRequest.Value);
        }

        [TestMethod]
        public async Task SpasiSliku_ValidnaSlikaSaPrefiksomINepostojeciFolder_KreiraFolderSpasavaFajlIPreusmjerava()
        {
            // Arrange
            byte[] testniBajtovi = Encoding.UTF8.GetBytes("lazni-jpeg-sadrzaj");
            string base64SaPrefiksom = "data:image/jpeg;base64," + Convert.ToBase64String(testniBajtovi);
            int narudzbaId = 7;

            string ocekivaniFolder = Path.Combine(_tempWebRootPath, "slike_torta");
            string ocekivaniFajl = Path.Combine(ocekivaniFolder, "bento_narudzba_7.jpg");
            Assert.IsFalse(Directory.Exists(ocekivaniFolder));

            // Act
            var rezultat = await _controller.SpasiSliku(base64SaPrefiksom, narudzbaId);

            // Assert
            Assert.IsTrue(Directory.Exists(ocekivaniFolder));
            Assert.IsTrue(File.Exists(ocekivaniFajl));
            CollectionAssert.AreEqual(testniBajtovi, await File.ReadAllBytesAsync(ocekivaniFajl));

            Assert.AreEqual(
                "Uspješno uslikana bento torta i poslana obavijest sa slikom na e-mail!",
                _controller.TempData["Poruka"]);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("AdminPanel", redirect.ActionName);
            Assert.AreEqual("Home", redirect.ControllerName);
        }

        [TestMethod]
        public async Task SpasiSliku_FolderVecPostojiIBezPrefiksa_PrepisePostojecuSliku()
        {
            // Arrange - unaprijed kreiramo folder da pokrijemo false granu od !Directory.Exists
            string postojeciFolder = Path.Combine(_tempWebRootPath, "slike_torta");
            Directory.CreateDirectory(postojeciFolder);

            byte[] noviBajtovi = Encoding.UTF8.GetBytes("nova-verzija-slike");
            string cistiBase64 = Convert.ToBase64String(noviBajtovi);
            int narudzbaId = 12;

            // Act
            var rezultat = await _controller.SpasiSliku(cistiBase64, narudzbaId);

            // Assert
            string ocekivaniFajl = Path.Combine(postojeciFolder, "bento_narudzba_12.jpg");
            Assert.IsTrue(File.Exists(ocekivaniFajl));
            CollectionAssert.AreEqual(noviBajtovi, await File.ReadAllBytesAsync(ocekivaniFajl));
            Assert.IsInstanceOfType(rezultat, typeof(RedirectToActionResult));
        }

        [TestMethod]
        public async Task SpasiSliku_NevalidanBase64Format_BacaFormatException()
        {
            // Arrange
            string neispravanBase64 = "data:image/jpeg;base64,OVO_NIJE_VALIDAN_BASE64@@@";

            // Act & Assert
            try
            {
                await _controller.SpasiSliku(neispravanBase64, 15);
                Assert.Fail("Očekivan je FormatException za neispravan Base64 string.");
            }
            catch (FormatException)
            {
                // Očekivani izuzetak
            }
        }
    }
}