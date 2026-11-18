using System.Text.Json;
using BentoLab.Controllers;
using BentoLab.Data;
using BentoLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestiranje
{
    [TestClass]
    public class KorisniksControllerTests
    {
        private ApplicationDbContext KreirajContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private Korisnik KreirajKorisnika(int id = 1)
        {
            return new Korisnik
            {
                Id = id,
                UserName = "korisnik" + id,
                Email = "korisnik" + id + "@bentolab.ba",
                ImePrezime = "Test Korisnik " + id,
                Uloga = Uloga.KUPAC
            };
        }

        // ---------------- INDEX ----------------

        [TestMethod]
        public async Task Index_PraznaBaza_VracaPraznuListu()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Index();

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            var model = view.Model as List<Korisnik>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count);
        }

        [TestMethod]
        public async Task Index_PostojeciPodaci_VracaSveKorisnike()
        {
            using var context = KreirajContext();
            context.Users.AddRange(KreirajKorisnika(1), KreirajKorisnika(2), KreirajKorisnika(3));
            await context.SaveChangesAsync();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Index();

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            var model = view.Model as List<Korisnik>;
            Assert.IsNotNull(model);
            Assert.AreEqual(3, model.Count);
        }

        // ---------------- DETAILS ----------------

        [TestMethod]
        public async Task Details_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Details(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Details_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Details(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Details_PostojeciID_VracaKorisnika()
        {
            using var context = KreirajContext();
            context.Users.Add(KreirajKorisnika(1));
            await context.SaveChangesAsync();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Details(1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            var model = view.Model as Korisnik;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Id);
            Assert.AreEqual("Test Korisnik 1", model.ImePrezime);
        }

        // ---------------- CREATE ----------------

        [TestMethod]
        public void Create_Get_VracaView()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);

            var rezultat = controller.Create();

            Assert.IsInstanceOfType<ViewResult>(rezultat);
        }

        [TestMethod]
        public async Task Create_ValidanModel_SnimaIPreusmjerava()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Create(KreirajKorisnika(5));

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual(1, await context.Users.CountAsync());
        }

        [TestMethod]
        public async Task Create_NevalidanModel_VracaViewINeSnima()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);
            controller.ModelState.AddModelError("Email", "Email je obavezan");

            var rezultat = await controller.Create(KreirajKorisnika(5));

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            Assert.AreEqual(0, await context.Users.CountAsync());
        }

        // ---------------- EDIT (GET) ----------------

        [TestMethod]
        public async Task Edit_Get_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Edit(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Get_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Edit(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Get_PostojeciID_VracaKorisnika()
        {
            using var context = KreirajContext();
            context.Users.Add(KreirajKorisnika(1));
            await context.SaveChangesAsync();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Edit(1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            Assert.IsInstanceOfType<Korisnik>(view.Model);
        }

        // ---------------- EDIT (POST) ----------------

        [TestMethod]
        public async Task Edit_Post_IDSeNePoklapa_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Edit(2, KreirajKorisnika(1));

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Post_ValidanModel_AzuriraIPreusmjerava()
        {
            using var context = KreirajContext();
            var original = KreirajKorisnika(1);
            context.Users.Add(original);
            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();

            var controller = new KorisniksController(context);
            var izmijenjen = KreirajKorisnika(1);
            izmijenjen.ConcurrencyStamp = original.ConcurrencyStamp;
            izmijenjen.ImePrezime = "Novo Ime";

            var rezultat = await controller.Edit(1, izmijenjen);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            context.ChangeTracker.Clear();
            var sacuvan = await context.Users.FirstOrDefaultAsync(k => k.Id == 1);
            Assert.IsNotNull(sacuvan);
            Assert.AreEqual("Novo Ime", sacuvan.ImePrezime);
        }

        [TestMethod]
        public async Task Edit_Post_NevalidanModel_VracaView()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);
            controller.ModelState.AddModelError("Email", "Email je obavezan");

            var rezultat = await controller.Edit(1, KreirajKorisnika(1));

            Assert.IsInstanceOfType<ViewResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Post_KorisnikNePostojiUBazi_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Edit(1, KreirajKorisnika(1));

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        // ---------------- DELETE ----------------

        [TestMethod]
        public async Task Delete_Get_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Delete(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Delete_Get_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Delete(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Delete_Get_PostojeciID_VracaKorisnika()
        {
            using var context = KreirajContext();
            context.Users.Add(KreirajKorisnika(1));
            await context.SaveChangesAsync();
            var controller = new KorisniksController(context);

            var rezultat = await controller.Delete(1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            Assert.IsInstanceOfType<Korisnik>(view.Model);
        }

        [TestMethod]
        public async Task DeleteConfirmed_PostojeciID_BriseKorisnika()
        {
            using var context = KreirajContext();
            context.Users.Add(KreirajKorisnika(1));
            await context.SaveChangesAsync();
            var controller = new KorisniksController(context);

            var rezultat = await controller.DeleteConfirmed(1);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual(0, await context.Users.CountAsync());
        }

        [TestMethod]
        public async Task DeleteConfirmed_NepostojeciID_SamoPreusmjerava()
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);

            var rezultat = await controller.DeleteConfirmed(999);

            Assert.IsInstanceOfType<RedirectToActionResult>(rezultat);
        }

        // ---------------- DATA-DRIVEN (CSV & JSON) ----------------

        public static IEnumerable<object[]> CSVPodaci
        {
            get
            {
                var putanja = Path.Combine(AppContext.BaseDirectory, "TestData", "korisnici_uloge.csv");
                var linije = File.ReadAllLines(putanja).Skip(1);
                foreach (var linija in linije)
                {
                    if (string.IsNullOrWhiteSpace(linija)) continue;
                    var dijelovi = linija.Split(',');
                    yield return new object[]
                    {
                        dijelovi[0].Trim(),
                        dijelovi[1].Trim(),
                        Enum.Parse<Uloga>(dijelovi[2].Trim())
                    };
                }
            }
        }

        public static IEnumerable<object[]> JSONPodaci
        {
            get
            {
                var putanja = Path.Combine(AppContext.BaseDirectory, "TestData", "korisnici_uloge.json");
                var json = File.ReadAllText(putanja);
                using var doc = JsonDocument.Parse(json);
                foreach (var element in doc.RootElement.EnumerateArray())
                {
                    yield return new object[]
                    {
                        element.GetProperty("ImePrezime").GetString()!,
                        element.GetProperty("Email").GetString()!,
                        Enum.Parse<Uloga>(element.GetProperty("Uloga").GetString()!)
                    };
                }
            }
        }

        [DataTestMethod]
        [DynamicData(nameof(CSVPodaci))]
        public async Task Create_PodaciIzDatoteke_CSV_DodajeKorisnika(string imePrezime, string email, Uloga uloga)
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);
            var korisnik = new Korisnik
            {
                UserName = email,
                Email = email,
                ImePrezime = imePrezime,
                Uloga = uloga
            };

            var rezultat = await controller.Create(korisnik);

            Assert.IsInstanceOfType<RedirectToActionResult>(rezultat);
            var sacuvan = await context.Users.FirstOrDefaultAsync(k => k.Email == email);
            Assert.IsNotNull(sacuvan);
            Assert.AreEqual(imePrezime, sacuvan.ImePrezime);
            Assert.AreEqual(uloga, sacuvan.Uloga);
        }

        [DataTestMethod]
        [DynamicData(nameof(JSONPodaci))]
        public async Task Create_PodaciIzDatoteke_JSON_DodajeKorisnika(string imePrezime, string email, Uloga uloga)
        {
            using var context = KreirajContext();
            var controller = new KorisniksController(context);
            var korisnik = new Korisnik
            {
                UserName = email,
                Email = email,
                ImePrezime = imePrezime,
                Uloga = uloga
            };

            var rezultat = await controller.Create(korisnik);

            Assert.IsInstanceOfType<RedirectToActionResult>(rezultat);
            var sacuvan = await context.Users.FirstOrDefaultAsync(k => k.Email == email);
            Assert.IsNotNull(sacuvan);
            Assert.AreEqual(imePrezime, sacuvan.ImePrezime);
            Assert.AreEqual(uloga, sacuvan.Uloga);
        }

        // ---------------- ZAMJENSKI OBJEKAT (MOCK) ----------------

        [TestMethod]
        public async Task Create_GreskaBaze_BacaDbUpdateException_Mock()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockContext = new Mock<ApplicationDbContext>(options) { CallBase = true };
            mockContext
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException("Simulirana greska pri snimanju korisnika"));

            var controller = new KorisniksController(mockContext.Object);

            await Assert.ThrowsExactlyAsync<DbUpdateException>(
                async () => await controller.Create(KreirajKorisnika(10))
            );

            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
