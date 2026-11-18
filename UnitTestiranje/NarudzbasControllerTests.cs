using System.Security.Claims;
using BentoLab.Controllers;
using BentoLab.Data;
using BentoLab.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestiranje
{
    [TestClass]
    public class NarudzbasControllerTests
    {
        private ApplicationDbContext KreirajContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private NarudzbasController KreirajController(ApplicationDbContext context, int? korisnikId = null)
        {
            var httpContext = new DefaultHttpContext();
            if (korisnikId.HasValue)
            {
                var claims = new[] { new Claim(ClaimTypes.NameIdentifier, korisnikId.Value.ToString()) };
                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            }

            return new NarudzbasController(context)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext }
            };
        }

        private Korisnik KreirajKorisnika(int id, string email)
        {
            return new Korisnik
            {
                Id = id,
                UserName = email,
                Email = email,
                ImePrezime = "Korisnik " + id,
                Uloga = Uloga.KUPAC
            };
        }

        // ---------------- INDEX ----------------

        [TestMethod]
        public async Task Index_PraznaBaza_VracaPraznuListu()
        {
            using var context = KreirajContext();
            var controller = KreirajController(context);

            var rezultat = await controller.Index() as ViewResult;

            Assert.IsNotNull(rezultat);
            var model = rezultat.Model as List<Narudzba>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count);
        }

        [TestMethod]
        public async Task Index_NarudzbaSaNultomCijenomIStavkama_RacunaUkupnuCijenuIzStavki()
        {
            using var context = KreirajContext();
            var korisnik = KreirajKorisnika(1, "kupac@bentolab.ba");
            var torta = new Torta { TortaID = 1, Naziv = "Bento Klasik", Cijena = 25, KolicinaNaStanju = 5, Dostupna = true };
            var narudzba = new Narudzba
            {
                NarudzbaID = 1,
                KorisnikID = 1,
                UkupnaCijena = 0,
                Status = StatusNarudzbe.KREIRANA,
                DatumNarudzbe = DateTime.UtcNow,
                DatumPreuzimanja = DateTime.UtcNow.AddDays(2),
                NacinPreuzimanja = NacinPreuzimanja.LICNO_PREUZIMANJE,
                Stavke = new List<StavkaNarudzbe>
                {
                    new StavkaNarudzbe { StavkaID = 1, TortaID = 1, Kolicina = 1, CijenaStavke = 25.0 },
                    new StavkaNarudzbe { StavkaID = 2, TortaID = 1, Kolicina = 1, CijenaStavke = 30.5 }
                }
            };

            context.Users.Add(korisnik);
            context.Torta.Add(torta);
            context.Narudzbe.Add(narudzba);
            await context.SaveChangesAsync();

            var controller = KreirajController(context);

            var rezultat = await controller.Index() as ViewResult;

            Assert.IsNotNull(rezultat);
            var model = rezultat.Model as List<Narudzba>;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Count);
            Assert.AreEqual(55.5, model[0].UkupnaCijena, 0.001);
        }

        // ---------------- CREATE (GET) ----------------

        [TestMethod]
        public void Create_Get_PostavljaTrenutniDatumIVracaView()
        {
            using var context = KreirajContext();
            var controller = KreirajController(context);

            var rezultat = controller.Create() as ViewResult;

            Assert.IsNotNull(rezultat);
            Assert.IsNotNull(controller.ViewBag.TrenutniDatum);
        }

        // ---------------- CREATE (POST) ----------------

        [TestMethod]
        public async Task Create_Post_PrijavljeniKorisnikIDostava_KreiraNarudzbuIPreusmjerava()
        {
            using var context = KreirajContext();
            context.Users.Add(KreirajKorisnika(5, "prijavljeni@bentolab.ba"));
            await context.SaveChangesAsync();

            var controller = KreirajController(context, korisnikId: 5);
            var datum = new DateTime(2026, 12, 15, 14, 0, 0);

            var rezultat = await controller.Create(datum, "DOSTAVA") as RedirectToActionResult;

            Assert.IsNotNull(rezultat);
            Assert.AreEqual("Index", rezultat.ActionName);

            var sacuvana = await context.Narudzbe.FirstOrDefaultAsync();
            Assert.IsNotNull(sacuvana);
            Assert.AreEqual(5, sacuvana.KorisnikID);
            Assert.AreEqual(NacinPreuzimanja.DOSTAVA, sacuvana.NacinPreuzimanja);
            Assert.AreEqual(StatusNarudzbe.KREIRANA, sacuvana.Status);
        }

        [TestMethod]
        public async Task Create_Post_BezSesije_PronalaziKorisnikaPoDefaultEmailu()
        {
            using var context = KreirajContext();
            context.Users.Add(KreirajKorisnika(12, "amerdzanic1@etf.unsa.ba"));
            await context.SaveChangesAsync();

            var controller = KreirajController(context);

            var rezultat = await controller.Create(DateTime.Now.AddDays(3), "LICNO_PREUZIMANJE") as RedirectToActionResult;

            Assert.IsNotNull(rezultat);
            Assert.AreEqual("Index", rezultat.ActionName);
            var sacuvana = await context.Narudzbe.FirstOrDefaultAsync();
            Assert.IsNotNull(sacuvana);
            Assert.AreEqual(12, sacuvana.KorisnikID);
            Assert.AreEqual(NacinPreuzimanja.LICNO_PREUZIMANJE, sacuvana.NacinPreuzimanja);
        }

        [TestMethod]
        public async Task Create_Post_BezSesijeIDefaultEmaila_UzimaPrvogKorisnikaIzBaze()
        {
            using var context = KreirajContext();
            context.Users.Add(KreirajKorisnika(99, "neki.drugi@bentolab.ba"));
            await context.SaveChangesAsync();

            var controller = KreirajController(context);

            var rezultat = await controller.Create(DateTime.Now.AddDays(1), "LICNO") as RedirectToActionResult;

            Assert.IsNotNull(rezultat);
            var sacuvana = await context.Narudzbe.FirstOrDefaultAsync();
            Assert.IsNotNull(sacuvana);
            Assert.AreEqual(99, sacuvana.KorisnikID);
        }

        [TestMethod]
        public async Task Create_Post_PraznaTabelaKorisnika_VracaContentGresku()
        {
            using var context = KreirajContext();
            var controller = KreirajController(context);

            var rezultat = await controller.Create(DateTime.Now.AddDays(1), "DOSTAVA") as ContentResult;

            Assert.IsNotNull(rezultat);
            Assert.IsTrue(rezultat.Content!.Contains("Tabela sa korisnicima (Users) je potpuno prazna"));
            Assert.AreEqual(0, await context.Narudzbe.CountAsync());
        }

        [TestMethod]
        public async Task Create_Post_GreskaBaze_HvataIzuzetakIVracaContentPoruku_Mock()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            var mockContext = new Mock<ApplicationDbContext>(options) { CallBase = true };
            mockContext.Object.Users.Add(KreirajKorisnika(1, "kupac@bentolab.ba"));
            await mockContext.Object.SaveChangesAsync();
            mockContext.Invocations.Clear();

            mockContext
                .Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .ThrowsAsync(new DbUpdateException("Simulirani prekid konekcije"));

            var controller = KreirajController(mockContext.Object, korisnikId: 1);

            var rezultat = await controller.Create(DateTime.Now.AddDays(2), "DOSTAVA") as ContentResult;

            Assert.IsNotNull(rezultat);
            Assert.IsTrue(rezultat.Content!.Contains("Greška prilikom kreiranja narudžbe"));
            mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}