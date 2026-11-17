
using BentoLab.Controllers;
using BentoLab.Data;
using BentoLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace UnitTestiranje
{
    [TestClass]
    public class StavkaNarudzbesControllerTests
    {
        private DbContextOptions<ApplicationDbContext> KreirajOpcije()
        {
            return new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
        }

        private ApplicationDbContext KreirajContext()
        {
            return new ApplicationDbContext(KreirajOpcije());
        }

        private Korisnik KreirajKorisnika()
        {
            return new Korisnik
            {
                Id = 1,
                UserName = "testkorisnik",
                Email = "test@test.com",
                ImePrezime = "Test Korisnik"
            };
        }

        private Narudzba KreirajNarudzbu(int id = 1)
        {
            return new Narudzba
            {
                NarudzbaID = id,
                KorisnikID = 1,
                UkupnaCijena = 50,
                KoeficijentSlozenosti = 1,
                Status = StatusNarudzbe.KREIRANA,
                DatumNarudzbe = new DateTime(2026, 11, 10),
                DatumPreuzimanja = new DateTime(2026, 11, 20),
                NacinPreuzimanja = NacinPreuzimanja.DOSTAVA
            };
        }

        private Torta KreirajTortu(int id = 1)
        {
            return new Torta
            {
                TortaID = id,
                Naziv = "Čokoladna torta",
                Cijena = 25,
                KolicinaNaStanju = 10,
                Dostupna = true
            };
        }

        private StavkaNarudzbe KreirajStavku(int id = 1)
        {
            return new StavkaNarudzbe
            {
                StavkaID = id,
                Kolicina = 2,
                CijenaStavke = 10,
                NarudzbaID = 1,
                TortaID = 1
            };
        }

        private async Task DodajPodatke(
            ApplicationDbContext context,
            bool dodajStavku = false,
            bool dodajDruguStavku = false)
        {
            context.Korisnik.Add(KreirajKorisnika());
            context.Narudzbe.Add(KreirajNarudzbu());
            context.Torta.Add(KreirajTortu());

            if (dodajStavku)
                context.StavkeNarudzbe.Add(KreirajStavku());

            if (dodajDruguStavku)
            {
                context.StavkeNarudzbe.Add(new StavkaNarudzbe
                {
                    StavkaID = 2,
                    Kolicina = 3,
                    CijenaStavke = 10,
                    NarudzbaID = 1,
                    TortaID = 1
                });
            }

            await context.SaveChangesAsync();
            context.ChangeTracker.Clear();
        }

        // ---------------- INDEX ----------------

        [TestMethod]
        public async Task Index_PraznaBaza_VracaPraznuListu()
        {
            using var context = KreirajContext();
            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Index();

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as System.Collections.Generic.List<StavkaNarudzbe>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count);
        }

        [TestMethod]
        public async Task Index_PostojeciPodaci_VracaSveStavke()
        {
            using var context = KreirajContext();
            await DodajPodatke(context, true, true);

            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Index();

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as System.Collections.Generic.List<StavkaNarudzbe>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
            Assert.IsNotNull(model[0].Narudzba);
            Assert.IsNotNull(model[0].Torta);
        }

        // ---------------- DETAILS ----------------

        [TestMethod]
        public async Task Details_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Details(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Details_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Details(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Details_PostojeciID_VracaStavku()
        {
            using var context = KreirajContext();
            await DodajPodatke(context, true);

            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Details(1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as StavkaNarudzbe;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.StavkaID);
            Assert.AreEqual(2, model.Kolicina);
            Assert.IsNotNull(model.Narudzba);
            Assert.IsNotNull(model.Torta);
        }

        // ---------------- CREATE GET ----------------

        [TestMethod]
        public async Task Create_Get_VracaPocetnuKolicinuIListuTorti()
        {
            using var context = KreirajContext();
            await DodajPodatke(context);

            var controller = new StavkaNarudzbesController(context);

            var rezultat = controller.Create();

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as StavkaNarudzbe;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.Kolicina);

            Assert.IsInstanceOfType<SelectList>(controller.ViewBag.TortaID);
        }

        // ---------------- CREATE POST ----------------

        [TestMethod]
        public async Task Create_NemaNarudzbe_PreusmjeravaNaKreiranjeNarudzbe()
        {
            using var context = KreirajContext();
            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Create(new StavkaNarudzbe
            {
                Kolicina = 2,
                TortaID = 1
            });

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Create", redirect.ActionName);
            Assert.AreEqual("Narudzbas", redirect.ControllerName);
        }

        [TestMethod]
        public async Task Create_NepostojecaTorta_VracaNotFound()
        {
            using var context = KreirajContext();
            await DodajPodatke(context);

            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Create(new StavkaNarudzbe
            {
                Kolicina = 2,
                TortaID = 999
            });

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Create_IspravniPodaci_DodajeStavkuIObracunavaCijenu()
        {
            using var context = KreirajContext();
            await DodajPodatke(context);

            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Create(new StavkaNarudzbe
            {
                Kolicina = 3,
                TortaID = 1
            });

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            var stavka = await context.StavkeNarudzbe.FirstOrDefaultAsync();
            Assert.IsNotNull(stavka);
            Assert.AreEqual(3, stavka.Kolicina);
            Assert.AreEqual(25, stavka.CijenaStavke);
            Assert.AreEqual(1, stavka.NarudzbaID);

            var narudzba = await context.Narudzbe.FindAsync(1);
            Assert.IsNotNull(narudzba);
            Assert.AreEqual(75, narudzba.UkupnaCijena);
        }

        [TestMethod]
        public async Task Create_NeispravanModel_VracaView()
        {
            using var context = KreirajContext();
            await DodajPodatke(context);

            var controller = new StavkaNarudzbesController(context);
            controller.ModelState.AddModelError("Kolicina", "Neispravna količina");

            var rezultat = await controller.Create(new StavkaNarudzbe
            {
                Kolicina = 0,
                TortaID = 1
            });

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            Assert.IsInstanceOfType<SelectList>(controller.ViewBag.TortaID);

            Assert.AreEqual(0, await context.StavkeNarudzbe.CountAsync());
        }

        // ---------------- EDIT GET ----------------

        [TestMethod]
        public async Task Edit_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Edit(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Edit(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_PostojeciID_VracaStavku()
        {
            using var context = KreirajContext();
            await DodajPodatke(context, true);

            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Edit(1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as StavkaNarudzbe;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.StavkaID);
            Assert.IsInstanceOfType<SelectList>(controller.ViewBag.TortaID);
        }

        // ---------------- EDIT POST ----------------

        [TestMethod]
        public async Task Edit_RazlicitiID_VracaNotFound()
        {
            using var context = KreirajContext();
            await DodajPodatke(context, true);

            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Edit(5, new StavkaNarudzbe
            {
                StavkaID = 1,
                Kolicina = 4,
                TortaID = 1,
                NarudzbaID = 1
            });

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_NepostojecaTorta_VracaNotFound()
        {
            using var context = KreirajContext();
            await DodajPodatke(context, true);

            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Edit(1, new StavkaNarudzbe
            {
                StavkaID = 1,
                Kolicina = 4,
                TortaID = 999,
                NarudzbaID = 1
            });

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_IspravniPodaci_MijenjaStavkuICijenuNarudzbe()
        {
            using var context = KreirajContext();
            await DodajPodatke(context, true);

            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Edit(1, new StavkaNarudzbe
            {
                StavkaID = 1,
                Kolicina = 4,
                TortaID = 1,
                NarudzbaID = 1
            });

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            var stavka = await context.StavkeNarudzbe.FindAsync(1);
            Assert.IsNotNull(stavka);
            Assert.AreEqual(4, stavka.Kolicina);
            Assert.AreEqual(25, stavka.CijenaStavke);

            var narudzba = await context.Narudzbe.FindAsync(1);
            Assert.IsNotNull(narudzba);
            Assert.AreEqual(100, narudzba.UkupnaCijena);
        }

        [TestMethod]
        public async Task Edit_NeispravanModel_VracaView()
        {
            using var context = KreirajContext();
            await DodajPodatke(context, true);

            var controller = new StavkaNarudzbesController(context);
            controller.ModelState.AddModelError("Kolicina", "Neispravna količina");

            var rezultat = await controller.Edit(1, new StavkaNarudzbe
            {
                StavkaID = 1,
                Kolicina = 0,
                TortaID = 1,
                NarudzbaID = 1
            });

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            Assert.IsInstanceOfType<SelectList>(controller.ViewBag.TortaID);

            var stavka = await context.StavkeNarudzbe.FindAsync(1);
            Assert.IsNotNull(stavka);
            Assert.AreEqual(2, stavka.Kolicina);
        }

        // ---------------- DELETE GET ----------------

        [TestMethod]
        public async Task Delete_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Delete(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Delete_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Delete(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Delete_PostojeciID_VracaStavku()
        {
            using var context = KreirajContext();
            await DodajPodatke(context, true);

            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.Delete(1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as StavkaNarudzbe;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.StavkaID);
            Assert.IsNotNull(model.Narudzba);
            Assert.IsNotNull(model.Torta);
        }

        // ---------------- DELETE POST ----------------

        [TestMethod]
        public async Task DeleteConfirmed_PostojecaStavka_BriseStavkuIPreracunavaCijenu()
        {
            using var context = KreirajContext();
            await DodajPodatke(context, true, true);

            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.DeleteConfirmed(1);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            Assert.IsNull(await context.StavkeNarudzbe.FindAsync(1));
            Assert.IsNotNull(await context.StavkeNarudzbe.FindAsync(2));

            var narudzba = await context.Narudzbe.FindAsync(1);
            Assert.IsNotNull(narudzba);
            Assert.AreEqual(30, narudzba.UkupnaCijena);
        }

        [TestMethod]
        public async Task DeleteConfirmed_NepostojecaStavka_VracaIndex()
        {
            using var context = KreirajContext();
            await DodajPodatke(context);

            var controller = new StavkaNarudzbesController(context);

            var rezultat = await controller.DeleteConfirmed(999);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
        }

        // ---------------- PRIVATE METODA ----------------

        [TestMethod]
        public async Task StavkaNarudzbeExists_PostojeciINepostojeciID_VracaTacnuVrijednost()
        {
            using var context = KreirajContext();
            await DodajPodatke(context, true);

            var controller = new StavkaNarudzbesController(context);

            var metoda = typeof(StavkaNarudzbesController).GetMethod(
                "StavkaNarudzbeExists",
                BindingFlags.NonPublic | BindingFlags.Instance);

            Assert.IsNotNull(metoda);

            var postoji = metoda.Invoke(controller, new object[] { 1 });
            var nePostoji = metoda.Invoke(controller, new object[] { 999 });

            Assert.AreEqual(true, postoji);
            Assert.AreEqual(false, nePostoji);
        }
    }
}
