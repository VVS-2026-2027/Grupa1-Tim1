
using BentoLab.Controllers;
using BentoLab.Data;
using BentoLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UnitTestiranje
{
    [TestClass]
    public class ObavjestenjesControllerTests
    {
        // ---------------- POMOCNE METODE ----------------

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

        private Korisnik KreirajKorisnika(int id = 1)
        {
            return new Korisnik
            {
                Id = id,
                UserName = $"korisnik{id}",
                Email = $"korisnik{id}@test.com",
                ImePrezime = "Test Korisnik"
            };
        }

        private Narudzba KreirajNarudzbu(int id = 1)
        {
            return new Narudzba
            {
                NarudzbaID = id,
                KorisnikID = 1,
                UkupnaCijena = 50.0,
                KoeficijentSlozenosti = 1.0,
                Status = StatusNarudzbe.KREIRANA,
                DatumNarudzbe = new DateTime(2026, 11, 10),
                DatumPreuzimanja = new DateTime(2026, 11, 20),
                NacinPreuzimanja = NacinPreuzimanja.DOSTAVA
            };
        }

        private Obavjestenje KreirajObavjestenje(int id = 1)
        {
            return new Obavjestenje
            {
                ObavjestenjeID = id,
                Sadrzaj = "Vaša narudžba je potvrđena.",
                DatumSlanja = new DateTime(2026, 11, 14),
                KorisnikID = 1,
                NarudzbaID = 1
            };
        }

        private async Task DodajPovezanePodatke(
            ApplicationDbContext context)
        {
            context.Korisnik.Add(KreirajKorisnika());
            context.Narudzbe.Add(KreirajNarudzbu());
            await context.SaveChangesAsync();
        }

        private async Task DodajObavjestenje(
            ApplicationDbContext context)
        {
            await DodajPovezanePodatke(context);
            context.Obavjestenja.Add(KreirajObavjestenje());
            await context.SaveChangesAsync();
        }

        // ---------------- INDEX ----------------

        [TestMethod]
        public async Task Index_PraznaBaza_VracaPraznuListu()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            var rezultat = await controller.Index();

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as List<Obavjestenje>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count);
        }

        [TestMethod]
        public async Task Index_PostojeciPodaci_VracaSvaObavjestenja()
        {
            using var context = KreirajContext();
            await DodajPovezanePodatke(context);

            context.Obavjestenja.AddRange(
                KreirajObavjestenje(1),
                KreirajObavjestenje(2));

            await context.SaveChangesAsync();

            var controller = new ObavjestenjesController(context);
            var rezultat = await controller.Index();

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as List<Obavjestenje>;
            Assert.IsNotNull(model);

            Assert.AreEqual(2, model.Count);
            Assert.IsTrue(model.All(o => o.Korisnik != null));
            Assert.IsTrue(model.All(o => o.Narudzba != null));
        }

        // ---------------- DETAILS ----------------

        [TestMethod]
        public async Task Details_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            var rezultat = await controller.Details(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Details_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            var rezultat = await controller.Details(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Details_PostojeciID_VracaObavjestenje()
        {
            using var context = KreirajContext();
            await DodajObavjestenje(context);

            var controller = new ObavjestenjesController(context);
            var rezultat = await controller.Details(1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as Obavjestenje;
            Assert.IsNotNull(model);

            Assert.AreEqual(1, model.ObavjestenjeID);
            Assert.AreEqual("Vaša narudžba je potvrđena.", model.Sadrzaj);
            Assert.IsNotNull(model.Korisnik);
            Assert.IsNotNull(model.Narudzba);
            Assert.AreEqual(1, model.Korisnik.Id);
            Assert.AreEqual(1, model.Narudzba.NarudzbaID);
        }

        // ---------------- CREATE GET ----------------

        [TestMethod]
        public void Create_Get_VracaView()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            var rezultat = controller.Create();

            Assert.IsInstanceOfType<ViewResult>(rezultat);
        }

        [TestMethod]
        public void Create_Get_PostavljaSelectListe()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            controller.Create();

            Assert.IsInstanceOfType<SelectList>(
                controller.ViewData["KorisnikID"]);

            Assert.IsInstanceOfType<SelectList>(
                controller.ViewData["NarudzbaID"]);
        }

        // ---------------- CREATE POST ----------------

        [TestMethod]
        public async Task Create_Post_ValidanModel_DodajeObavjestenje()
        {
            using var context = KreirajContext();
            await DodajPovezanePodatke(context);

            var controller = new ObavjestenjesController(context);
            var obavjestenje = KreirajObavjestenje();

            var rezultat = await controller.Create(obavjestenje);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            Assert.AreEqual(1, await context.Obavjestenja.CountAsync());
        }

        [TestMethod]
        public async Task Create_Post_NevalidanModel_VracaView()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            var obavjestenje = KreirajObavjestenje();

            controller.ModelState.AddModelError(
                "Sadrzaj", "Sadržaj nije validan");

            var rezultat = await controller.Create(obavjestenje);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            Assert.AreSame(obavjestenje, view.Model);

            Assert.AreEqual(0, await context.Obavjestenja.CountAsync());
        }

        [TestMethod]
        public async Task Create_Post_NevalidanModel_PostavljaSelectListe()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            controller.ModelState.AddModelError(
                "Sadrzaj", "Obavezno polje");

            await controller.Create(KreirajObavjestenje());

            Assert.IsInstanceOfType<SelectList>(
                controller.ViewData["KorisnikID"]);

            Assert.IsInstanceOfType<SelectList>(
                controller.ViewData["NarudzbaID"]);
        }

        // ---------------- EDIT GET ----------------

        [TestMethod]
        public async Task Edit_Get_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            var rezultat = await controller.Edit((int?)null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Get_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            var rezultat = await controller.Edit((int?)999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Get_PostojeciID_VracaObavjestenje()
        {
            using var context = KreirajContext();
            await DodajObavjestenje(context);

            var controller = new ObavjestenjesController(context);
            var rezultat = await controller.Edit((int?)1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as Obavjestenje;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.ObavjestenjeID);
        }

        [TestMethod]
        public async Task Edit_Get_PostojeciID_PostavljaSelectListe()
        {
            using var context = KreirajContext();
            await DodajObavjestenje(context);

            var controller = new ObavjestenjesController(context);

            await controller.Edit((int?)1);

            Assert.IsInstanceOfType<SelectList>(
                controller.ViewData["KorisnikID"]);

            Assert.IsInstanceOfType<SelectList>(
                controller.ViewData["NarudzbaID"]);
        }

        // ---------------- EDIT POST ----------------

        [TestMethod]
        public async Task Edit_Post_RazlicitiIDevi_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            var rezultat = await controller.Edit(
                2, KreirajObavjestenje(1));

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Post_ValidanModel_AzuriraObavjestenje()
        {
            using var context = KreirajContext();
            await DodajObavjestenje(context);

            context.ChangeTracker.Clear();

            var controller = new ObavjestenjesController(context);

            var izmijenjeno = KreirajObavjestenje();
            izmijenjeno.Sadrzaj = "Narudžba je spremna za preuzimanje.";

            var rezultat = await controller.Edit(1, izmijenjeno);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            context.ChangeTracker.Clear();

            var sacuvano = await context.Obavjestenja.FindAsync(1);

            Assert.IsNotNull(sacuvano);
            Assert.AreEqual(
                "Narudžba je spremna za preuzimanje.",
                sacuvano.Sadrzaj);
        }

        [TestMethod]
        public async Task Edit_Post_NevalidanModel_VracaView()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            var obavjestenje = KreirajObavjestenje();

            controller.ModelState.AddModelError(
                "Sadrzaj", "Nevalidan sadržaj");

            var rezultat = await controller.Edit(1, obavjestenje);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            Assert.AreSame(obavjestenje, view.Model);
        }

        [TestMethod]
        public async Task Edit_Post_NevalidanModel_PostavljaSelectListe()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            controller.ModelState.AddModelError(
                "Sadrzaj", "Nevalidan sadržaj");

            await controller.Edit(1, KreirajObavjestenje());

            Assert.IsInstanceOfType<SelectList>(
                controller.ViewData["KorisnikID"]);

            Assert.IsInstanceOfType<SelectList>(
                controller.ViewData["NarudzbaID"]);
        }

        // ---------------- DELETE GET ----------------

        [TestMethod]
        public async Task Delete_Get_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            var rezultat = await controller.Delete(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Delete_Get_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            var rezultat = await controller.Delete(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Delete_Get_PostojeciID_VracaObavjestenje()
        {
            using var context = KreirajContext();
            await DodajObavjestenje(context);

            var controller = new ObavjestenjesController(context);
            var rezultat = await controller.Delete(1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as Obavjestenje;
            Assert.IsNotNull(model);

            Assert.AreEqual(1, model.ObavjestenjeID);
            Assert.IsNotNull(model.Korisnik);
            Assert.IsNotNull(model.Narudzba);
        }


        [TestMethod]
        public async Task DeleteConfirmed_PostojeciID_BriseObavjestenje()
        {
            using var context = KreirajContext();
            await DodajObavjestenje(context);

            var controller = new ObavjestenjesController(context);

            var rezultat = await controller.DeleteConfirmed(1);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            Assert.AreEqual(0, await context.Obavjestenja.CountAsync());
        }

        [TestMethod]
        public async Task DeleteConfirmed_NepostojeciID_VracaRedirect()
        {
            using var context = KreirajContext();
            var controller = new ObavjestenjesController(context);

            var rezultat = await controller.DeleteConfirmed(999);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            Assert.AreEqual(0, await context.Obavjestenja.CountAsync());
        }


        [TestMethod]
        public async Task Edit_Post_KonfliktINepostojeceObavjestenje_VracaNotFound()
        {
            var options = KreirajOpcije();

            using var context = new ObavjestenjeContextSaKonfliktom(options);
            var controller = new ObavjestenjesController(context);

            var rezultat = await controller.Edit(
                999, KreirajObavjestenje(999));

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Post_KonfliktIPostojeceObavjestenje_BacaIzuzetak()
        {
            var options = KreirajOpcije();

            using (var context = new ApplicationDbContext(options))
            {
                await DodajObavjestenje(context);
            }

            using var konfliktContext =
                new ObavjestenjeContextSaKonfliktom(options);

            var controller = new ObavjestenjesController(konfliktContext);

            await Assert.ThrowsExactlyAsync<DbUpdateConcurrencyException>(
                async () => await controller.Edit(
                    1, KreirajObavjestenje(1)));
        }
    }


    public class ObavjestenjeContextSaKonfliktom : ApplicationDbContext
    {
        public ObavjestenjeContextSaKonfliktom(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            throw new DbUpdateConcurrencyException(
                "Simulirani konflikt pri spremanju obavjestenja.");
        }
    }
}
