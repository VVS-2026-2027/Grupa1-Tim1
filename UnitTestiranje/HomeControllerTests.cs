using BentoLab.Controllers;
using BentoLab.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnitTestiranje
{
    [TestClass]
    public class HomeControllerTests
    {
        private Mock<UserManager<Korisnik>> _mockUserManager = null!;
        private HomeController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            var store = new Mock<IUserStore<Korisnik>>();
            _mockUserManager = new Mock<UserManager<Korisnik>>(
                store.Object, null!, null!, null!, null!, null!, null!, null!, null!);

            _controller = new HomeController(_mockUserManager.Object);

            // Simuliramo HttpContext i prijavljenog korisnika (User)
            var httpContext = new DefaultHttpContext
            {
                TraceIdentifier = "test-trace-id-001",
                User = new ClaimsPrincipal(new ClaimsIdentity(
                    new[] { new Claim(ClaimTypes.NameIdentifier, "1") }, "TestAuth"))
            };

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [TestCleanup]
        public void Cleanup()
        {
            _controller?.Dispose();
        }

        private static string PronadjiTestDataFajl(string nazivFajla)
        {
            string putanjaIzProjekta = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "TestData", nazivFajla));
            if (File.Exists(putanjaIzProjekta))
                return putanjaIzProjekta;

            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestData", nazivFajla);
        }

        // 1. TESTIRANJE INDEX AKCIJE
        [TestMethod]
        public void Index_PozivAkcije_VracaViewResult()
        {
            // Act
            var rezultat = _controller.Index();

            // Assert
            Assert.IsInstanceOfType(rezultat, typeof(ViewResult));
        }

        // 2. DATA-DRIVEN TEST (JSON FORMAT) ZA PRISTUP ADMIN PANELU
        public static IEnumerable<object[]> UcitajJsonPodatkeZaPristup()
        {
            string putanja = PronadjiTestDataFajl("pristup_uloge.json");
            string json = File.ReadAllText(putanja);
            using var doc = JsonDocument.Parse(json);

            foreach (var element in doc.RootElement.EnumerateArray())
            {
                yield return new object[]
                {
                    element.GetProperty("Opis").GetString()!,
                    element.GetProperty("Postoji").GetBoolean(),
                    element.GetProperty("Uloga").GetString()!,
                    element.GetProperty("DozvoljenPristup").GetBoolean()
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(UcitajJsonPodatkeZaPristup), DynamicDataSourceType.Method)]
        public async Task AdminPanel_RazliciteUlogeIzJsonFajla_VracaOdgovarajuciRezultat(
            string opis, bool postoji, string ulogaTekst, bool dozvoljenPristup)
        {
            // Arrange
            Korisnik? korisnik = postoji
                ? new Korisnik { Id = 1, Uloga = Enum.Parse<Uloga>(ulogaTekst) }
                : null;

            _mockUserManager
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(korisnik);

            // Act
            var rezultat = await _controller.AdminPanel();

            // Assert
            if (dozvoljenPristup)
            {
                Assert.IsInstanceOfType(rezultat, typeof(ViewResult), $"Neuspjeh za scenarij: {opis}");
            }
            else
            {
                var redirect = rezultat as RedirectToActionResult;
                Assert.IsNotNull(redirect, $"Očekivan redirect za scenarij: {opis}");
                Assert.AreEqual("Index", redirect.ActionName);
                Assert.AreEqual("Torta", redirect.ControllerName);
            }
        }

        // 3. TESTIRANJE UPRAVLJANJA KORISNICIMA
        [TestMethod]
        public async Task UpravljanjeKorisnicima_KorisnikNijeAdmin_PreusmjeravaNaTortaIndex()
        {
            // Arrange
            _mockUserManager
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new Korisnik { Id = 2, Uloga = Uloga.KUPAC });

            // Act
            var rezultat = await _controller.UpravljanjeKorisnicima();

            // Assert
            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual("Torta", redirect.ControllerName);
        }

        [TestMethod]
        public async Task UpravljanjeKorisnicima_PrijavljenAdmin_VracaSamoKupceSortiraneOpadajuce()
        {
            // Arrange
            _mockUserManager
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new Korisnik { Id = 1, Uloga = Uloga.ADMIN });

            var listaKorisnika = new List<Korisnik>
            {
                new Korisnik { Id = 5, Uloga = Uloga.KUPAC, Email = "kupac1@test.ba" },
                new Korisnik { Id = 1, Uloga = Uloga.ADMIN, Email = "admin@test.ba" },
                new Korisnik { Id = 12, Uloga = Uloga.KUPAC, Email = "kupac2@test.ba" }
            }.AsQueryable();

            _mockUserManager.Setup(um => um.Users).Returns(listaKorisnika);

            // Act
            var rezultat = await _controller.UpravljanjeKorisnicima();

            // Assert
            var viewResult = rezultat as ViewResult;
            Assert.IsNotNull(viewResult);
            var model = viewResult.Model as List<Korisnik>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
            Assert.AreEqual(12, model[0].Id); // Provjera OrderByDescending
            Assert.AreEqual(5, model[1].Id);
        }

        // 4. TESTIRANJE ERROR AKCIJE (OBJE GRANE ZA RequestId)
        [TestMethod]
        public void Error_BezAktivnogActivityja_KoristiHttpContextTraceIdentifier()
        {
            // Arrange
            Activity.Current = null;

            // Act
            var rezultat = _controller.Error() as ViewResult;

            // Assert
            Assert.IsNotNull(rezultat);
            var model = rezultat.Model as ErrorViewModel;
            Assert.IsNotNull(model);
            Assert.AreEqual("test-trace-id-001", model.RequestId);
        }

        [TestMethod]
        public void Error_SaAktivnimActivityjem_KoristiActivityId()
        {
            // Arrange
            var activity = new Activity("TestOperacija");
            activity.Start();

            try
            {
                // Act
                var rezultat = _controller.Error() as ViewResult;

                // Assert
                Assert.IsNotNull(rezultat);
                var model = rezultat.Model as ErrorViewModel;
                Assert.IsNotNull(model);
                Assert.AreEqual(activity.Id, model.RequestId);
            }
            finally
            {
                activity.Stop();
            }
        }

        // 5. DATA-DRIVEN TEST (CSV FORMAT) ZA BLOKIRANJE KORISNIKA
        public static IEnumerable<object[]> UcitajCsvPodatkeZaBlokiranje()
        {
            string putanja = PronadjiTestDataFajl("blokiranje_korisnika.csv");
            var linije = File.ReadAllLines(putanja).Skip(1); // Preskačemo zaglavlje

            foreach (var linija in linije)
            {
                if (string.IsNullOrWhiteSpace(linija)) continue;
                var kolone = linija.Split(',');
                yield return new object[]
                {
                    int.Parse(kolone[0]),
                    bool.Parse(kolone[1]),
                    kolone[2].Trim(),
                    bool.Parse(kolone[3])
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(UcitajCsvPodatkeZaBlokiranje), DynamicDataSourceType.Method)]
        public async Task BlokirajKorisnika_PodaciIzCsvFajla_IspravnoPostavljaLockout(
            int idKorisnika, bool postojiUBazi, string ulogaTekst, bool trebaBlokirati)
        {
            // Arrange
            _mockUserManager
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new Korisnik { Id = 1, Uloga = Uloga.ADMIN });

            Korisnik? ciljniKorisnik = postojiUBazi
                ? new Korisnik { Id = idKorisnika, Uloga = Enum.Parse<Uloga>(ulogaTekst) }
                : null;

            _mockUserManager
                .Setup(um => um.FindByIdAsync(idKorisnika.ToString()))
                .ReturnsAsync(ciljniKorisnik);

            // Act
            var rezultat = await _controller.BlokirajKorisnika(idKorisnika) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(rezultat);
            Assert.AreEqual(nameof(HomeController.UpravljanjeKorisnicima), rezultat.ActionName);

            Times ocekivaniPoziv = trebaBlokirati ? Times.Once() : Times.Never();
            _mockUserManager.Verify(um => um.SetLockoutEnabledAsync(It.IsAny<Korisnik>(), true), ocekivaniPoziv);
            _mockUserManager.Verify(um => um.SetLockoutEndDateAsync(It.IsAny<Korisnik>(), It.IsAny<DateTimeOffset?>()), ocekivaniPoziv);
        }

        [TestMethod]
        public async Task BlokirajKorisnika_NijeAdmin_PreusmjeravaNaTortaIndex()
        {
            // Arrange
            _mockUserManager
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((Korisnik?)null);

            // Act
            var rezultat = await _controller.BlokirajKorisnika(10) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(rezultat);
            Assert.AreEqual("Index", rezultat.ActionName);
            Assert.AreEqual("Torta", rezultat.ControllerName);
        }

        // 6. TESTIRANJE ODBLOKIRANJA KORISNIKA
        [TestMethod]
        public async Task OdblokirajKorisnika_NijeAdmin_PreusmjeravaNaTortaIndex()
        {
            // Arrange
            _mockUserManager
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new Korisnik { Id = 2, Uloga = Uloga.KUPAC });

            // Act
            var rezultat = await _controller.OdblokirajKorisnika(10) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(rezultat);
            Assert.AreEqual("Index", rezultat.ActionName);
            Assert.AreEqual("Torta", rezultat.ControllerName);
        }

        [TestMethod]
        [DataRow(10, true, "KUPAC", true)]
        [DataRow(20, true, "ADMIN", false)]
        [DataRow(99, false, "KUPAC", false)]
        public async Task OdblokirajKorisnika_RazlicitiScenariji_IspravnoUklanjaLockout(
            int idKorisnika, bool postoji, string ulogaTekst, bool trebaOdblokirati)
        {
            // Arrange
            _mockUserManager
                .Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(new Korisnik { Id = 1, Uloga = Uloga.ADMIN });

            Korisnik? ciljniKorisnik = postoji
                ? new Korisnik { Id = idKorisnika, Uloga = Enum.Parse<Uloga>(ulogaTekst) }
                : null;

            _mockUserManager
                .Setup(um => um.FindByIdAsync(idKorisnika.ToString()))
                .ReturnsAsync(ciljniKorisnik);

            // Act
            var rezultat = await _controller.OdblokirajKorisnika(idKorisnika) as RedirectToActionResult;

            // Assert
            Assert.IsNotNull(rezultat);
            Assert.AreEqual(nameof(HomeController.UpravljanjeKorisnicima), rezultat.ActionName);

            Times ocekivaniPoziv = trebaOdblokirati ? Times.Once() : Times.Never();
            _mockUserManager.Verify(um => um.SetLockoutEndDateAsync(It.IsAny<Korisnik>(), null), ocekivaniPoziv);
        }
    }
}