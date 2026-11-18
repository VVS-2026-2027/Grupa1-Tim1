using System.Security.Claims;
using BentoLab.Controllers;
using BentoLab.Data;
using BentoLab.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestiranje
{
    [TestClass]
    public class KorpasControllerTests
    {
        private class MemorijaTempDataProvider : ITempDataProvider
        {
            private readonly Dictionary<string, object> _podaci = new();
            public IDictionary<string, object> LoadTempData(HttpContext context) => _podaci;
            public void SaveTempData(HttpContext context, IDictionary<string, object> values)
            {
                _podaci.Clear();
                foreach (var kv in values) _podaci[kv.Key] = kv.Value;
            }
        }

        private ApplicationDbContext KreirajContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        private KorpasController KreirajController(ApplicationDbContext context, int? korisnikId = null)
        {
            var httpContext = new DefaultHttpContext();
            if (korisnikId.HasValue)
            {
                var claims = new[] { new Claim(ClaimTypes.NameIdentifier, korisnikId.Value.ToString()) };
                httpContext.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "TestAuth"));
            }

            var controller = new KorpasController(context)
            {
                ControllerContext = new ControllerContext { HttpContext = httpContext },
                TempData = new TempDataDictionary(httpContext, new MemorijaTempDataProvider())
            };

            return controller;
        }

        [TestMethod]
        public void Index_VracaView()
        {
            using var context = KreirajContext();
            var controller = KreirajController(context);

            var rezultat = controller.Index();

            Assert.IsInstanceOfType<ViewResult>(rezultat);
        }

        [TestMethod]
        public void Create_SaStavkamaUTempData_RacunaMedjuzbir()
        {
            using var context = KreirajContext();
            var controller = KreirajController(context);
            controller.TempData["NazivKreiraneTorte"] = new[] { "Choco Bento", "Vanilla Bento" };
            controller.TempData["CijenaKreiraneTorte"] = new[] { "25.50", "30.00" };

            var rezultat = controller.Create() as ViewResult;

            Assert.IsNotNull(rezultat);
            Assert.AreEqual("55.50", controller.ViewBag.Medjuzbir);
        }

        [TestMethod]
        public void Create_PraznaKorpa_MedjuzbirJeNula()
        {
            using var context = KreirajContext();
            var controller = KreirajController(context);

            var rezultat = controller.Create() as ViewResult;

            Assert.IsNotNull(rezultat);
            Assert.AreEqual("0.00", controller.ViewBag.Medjuzbir);
        }

        [TestMethod]
        public void ProcesirajPreuzimanje_Dostava_PreusmjeravaNaDostavu()
        {
            using var context = KreirajContext();
            var controller = KreirajController(context);
            var datum = new DateTime(2026, 12, 1);

            var rezultat = controller.ProcesirajPreuzimanje("Dostava", "kupac@bentolab.ba", datum) as RedirectToActionResult;

            Assert.IsNotNull(rezultat);
            Assert.AreEqual("Dostava", rezultat.ActionName);
            Assert.AreEqual("2026-12-01", controller.TempData["DatumPreuzimanja"]);
        }

        [TestMethod]
        public void ProcesirajPreuzimanje_LicnoPreuzimanje_KreiraNarudzbuIStavke()
        {
            using var context = KreirajContext();
            context.Users.Add(new Korisnik
            {
                Id = 1,
                UserName = "kupac1",
                Email = "kupac1@bentolab.ba",
                ImePrezime = "Kupac Jedan",
                Uloga = Uloga.KUPAC
            });
            context.SaveChanges();

            var controller = KreirajController(context, korisnikId: 1);
            controller.TempData["NazivKreiraneTorte"] = new[] { "Jagoda Bento", "Cokolada Bento" };
            controller.TempData["CijenaKreiraneTorte"] = new[] { "20.00", "35.00" };

            var rezultat = controller.ProcesirajPreuzimanje("Licno", "", DateTime.Now.AddDays(2)) as RedirectToActionResult;

            Assert.IsNotNull(rezultat);
            Assert.AreEqual("Uspjeh", rezultat.ActionName);
            Assert.AreEqual(1, context.Narudzbe.Count());
            Assert.AreEqual(2, context.StavkeNarudzbe.Count());
            Assert.AreEqual(2, context.Torta.Count());

            var narudzba = context.Narudzbe.First();
            Assert.AreEqual(55.00, narudzba.UkupnaCijena, 0.001);
            Assert.AreEqual(NacinPreuzimanja.LICNO_PREUZIMANJE, narudzba.NacinPreuzimanja);
            Assert.AreEqual(StatusNarudzbe.KREIRANA, narudzba.Status);
        }

        [TestMethod]
        public void Dostava_VracaView()
        {
            using var context = KreirajContext();
            var controller = KreirajController(context);

            var rezultat = controller.Dostava();

            Assert.IsInstanceOfType<ViewResult>(rezultat);
        }

        [TestMethod]
        public void PotvrdiDostavu_SaStavkama_DodajeCijenuDostaveIKreiraNarudzbu()
        {
            using var context = KreirajContext();
            context.Users.Add(new Korisnik
            {
                Id = 2,
                UserName = "kupac2",
                Email = "kupac2@bentolab.ba",
                ImePrezime = "Kupac Dva",
                Uloga = Uloga.KUPAC
            });
            context.SaveChanges();

            // Namjerno bez korisnikId u claim-u da se testira fallback na prvog korisnika u bazi
            var controller = KreirajController(context);
            controller.TempData["DatumPreuzimanja"] = "2026-12-10";
            controller.TempData["NazivKreiraneTorte"] = new[] { "Pistacija Bento" };
            controller.TempData["CijenaKreiraneTorte"] = new[] { "40.00" };

            var rezultat = controller.PotvrdiDostavu("Ana", "Zmaja od Bosne 1", "Sarajevo", "061111222", "Bez oraha", "") as RedirectToActionResult;

            Assert.IsNotNull(rezultat);
            Assert.AreEqual("Uspjeh", rezultat.ActionName);
            var narudzba = context.Narudzbe.FirstOrDefault();
            Assert.IsNotNull(narudzba);
            Assert.AreEqual(45.00, narudzba.UkupnaCijena, 0.001); // 40 + 5 dostava
            Assert.AreEqual(NacinPreuzimanja.DOSTAVA, narudzba.NacinPreuzimanja);
            Assert.AreEqual(2, narudzba.KorisnikID);
        }

        [TestMethod]
        public void PotvrdiDostavu_PraznaKorpa_NeKreiraNarudzbu()
        {
            using var context = KreirajContext();
            var controller = KreirajController(context);

            var rezultat = controller.PotvrdiDostavu("Ana", "Adresa", "Sarajevo", "061000", "", "") as RedirectToActionResult;

            Assert.IsNotNull(rezultat);
            Assert.AreEqual("Uspjeh", rezultat.ActionName);
            Assert.AreEqual(0, context.Narudzbe.Count());
        }

        [TestMethod]
        public void Ukloni_ValidanIndex_UklanjaStavkuIzTempData()
        {
            using var context = KreirajContext();
            var controller = KreirajController(context);
            controller.TempData["NazivKreiraneTorte"] = new[] { "Torta 1", "Torta 2" };
            controller.TempData["CijenaKreiraneTorte"] = new[] { "15.00", "25.00" };

            var rezultat = controller.Ukloni(0) as RedirectToActionResult;

            Assert.IsNotNull(rezultat);
            Assert.AreEqual("Index", rezultat.ActionName);
            var preostaliNazivi = controller.TempData["NazivKreiraneTorte"] as string[];
            Assert.IsNotNull(preostaliNazivi);
            Assert.AreEqual(1, preostaliNazivi.Length);
            Assert.AreEqual("Torta 2", preostaliNazivi[0]);
        }

        [TestMethod]
        public void Ukloni_NevalidanIndex_NeMijenjaListu()
        {
            using var context = KreirajContext();
            var controller = KreirajController(context);
            controller.TempData["NazivKreiraneTorte"] = new[] { "Torta 1" };
            controller.TempData["CijenaKreiraneTorte"] = new[] { "15.00" };

            var rezultat = controller.Ukloni(5) as RedirectToActionResult;

            Assert.IsNotNull(rezultat);
            var nazivi = controller.TempData["NazivKreiraneTorte"] as string[];
            Assert.AreEqual(1, nazivi!.Length);
        }

        [TestMethod]
        public void Uspjeh_VracaView()
        {
            using var context = KreirajContext();
            var controller = KreirajController(context);

            var rezultat = controller.Uspjeh();

            Assert.IsInstanceOfType<ViewResult>(rezultat);
        }

        [TestMethod]
        public void PreuzmiIocistiKorpu_SaPodacima_VracaStavkeIBriseTempData()
        {
            var tempData = new TempDataDictionary(new DefaultHttpContext(), new MemorijaTempDataProvider())
            {
                ["NazivKreiraneTorte"] = new[] { "Bento A", "Bento B" },
                ["CijenaKreiraneTorte"] = new[] { "12.50", "18.00" }
            };

            var rezultat = KorpasController.PreuzmiIocistiKorpu(tempData);

            Assert.AreEqual(2, rezultat.Count);
            Assert.IsNull(tempData["NazivKreiraneTorte"]);
            Assert.IsNull(tempData["CijenaKreiraneTorte"]);
        }

        [TestMethod]
        public void PreuzmiIocistiKorpu_NullTempData_VracaPraznuListu()
        {
            var rezultat = KorpasController.PreuzmiIocistiKorpu(null!);

            Assert.IsNotNull(rezultat);
            Assert.AreEqual(0, rezultat.Count);
        }
    }
}