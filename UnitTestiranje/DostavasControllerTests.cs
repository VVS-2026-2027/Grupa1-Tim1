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
    public class DostavasControllerTests
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

        private Dostava KreirajDostavu(int id = 1)
        {
            return new Dostava
            {
                DostavaID = id,
                Adresa = "Sarajevo, Zmaja od Bosne 10",
                KontaktTelefon = "061123456",
                CijenaDostave = 5.0,
                VrijemeIsporuke = new DateTime(2026, 11, 20, 12, 0, 0),
                Napomena = "Pozvati prije dolaska",
                NarudzbaID = 1
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


        // ---------------- INDEX ----------------

        [TestMethod]
        public async Task Index_PraznaBaza_VracaPraznuListu()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var rezultat = await controller.Index();

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as List<Dostava>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count);
        }

        [TestMethod]
   public async Task Index_PostojeciPodaci_VracaSveDostave()
{
    using var context = KreirajContext();

    var narudzba1 = KreirajNarudzbu(1);
    var narudzba2 = KreirajNarudzbu(2);

    context.Narudzbe.AddRange(narudzba1, narudzba2);

    var dostava1 = KreirajDostavu(1);
    dostava1.NarudzbaID = 1;

    var dostava2 = KreirajDostavu(2);
    dostava2.NarudzbaID = 2;

    context.Dostave.AddRange(dostava1, dostava2);
    await context.SaveChangesAsync();

    var controller = new DostavasController(context);

    var rezultat = await controller.Index();

    var view = rezultat as ViewResult;
    Assert.IsNotNull(view);

    var model = view.Model as List<Dostava>;
    Assert.IsNotNull(model);

    Assert.AreEqual(2, model.Count);
    Assert.IsTrue(model.All(d => d.Narudzba != null));
}


        // ---------------- DETAILS ----------------

        [TestMethod]
        public async Task Details_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var rezultat = await controller.Details(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Details_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var rezultat = await controller.Details(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
public async Task Details_PostojeciID_VracaDostavu()
{
    using var context = KreirajContext();

    context.Narudzbe.Add(KreirajNarudzbu(1));
    context.Dostave.Add(KreirajDostavu(1));

    await context.SaveChangesAsync();

    var controller = new DostavasController(context);

    var rezultat = await controller.Details(1);

    var view = rezultat as ViewResult;
    Assert.IsNotNull(view);

    var model = view.Model as Dostava;
    Assert.IsNotNull(model);

    Assert.AreEqual(1, model.DostavaID);
    Assert.AreEqual("Sarajevo, Zmaja od Bosne 10", model.Adresa);
    Assert.IsNotNull(model.Narudzba);
    Assert.AreEqual(1, model.Narudzba.NarudzbaID);
}

        // ---------------- CREATE GET ----------------

        [TestMethod]
        public void Create_Get_VracaView()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var rezultat = controller.Create();

            Assert.IsInstanceOfType<ViewResult>(rezultat);
        }

        [TestMethod]
        public void Create_Get_PostavljaSelectList()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            controller.Create();

            Assert.IsNotNull(controller.ViewData["NarudzbaID"]);
            Assert.IsInstanceOfType<SelectList>(
                controller.ViewData["NarudzbaID"]);
        }

        // ---------------- CREATE POST ----------------

        [TestMethod]
        public async Task Create_Post_ValidanModel_DodajeDostavu()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var dostava = KreirajDostavu();

            var rezultat = await controller.Create(dostava);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            Assert.AreEqual(1, await context.Dostave.CountAsync());
        }

        [TestMethod]
        public async Task Create_Post_NevalidanModel_VracaView()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var dostava = KreirajDostavu();

            controller.ModelState.AddModelError(
                "Adresa", "Adresa nije validna");

            var rezultat = await controller.Create(dostava);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            Assert.AreSame(dostava, view.Model);

            Assert.AreEqual(0, await context.Dostave.CountAsync());
        }

        [TestMethod]
        public async Task Create_Post_NevalidanModel_PostavljaSelectList()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            controller.ModelState.AddModelError(
                "Adresa", "Obavezno polje");

            await controller.Create(KreirajDostavu());

            Assert.IsInstanceOfType<SelectList>(
                controller.ViewData["NarudzbaID"]);
        }

        // ---------------- EDIT GET ----------------

        [TestMethod]
        public async Task Edit_Get_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var rezultat = await controller.Edit((int?)null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Get_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var rezultat = await controller.Edit((int?)999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Get_PostojeciID_VracaDostavu()
        {
            using var context = KreirajContext();

            context.Dostave.Add(KreirajDostavu());
            await context.SaveChangesAsync();

            var controller = new DostavasController(context);

            var rezultat = await controller.Edit((int?)1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as Dostava;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.DostavaID);
        }

        [TestMethod]
        public async Task Edit_Get_PostojeciID_PostavljaSelectList()
        {
            using var context = KreirajContext();

            context.Dostave.Add(KreirajDostavu());
            await context.SaveChangesAsync();

            var controller = new DostavasController(context);

            await controller.Edit((int?)1);

            Assert.IsInstanceOfType<SelectList>(
                controller.ViewData["NarudzbaID"]);
        }

        // ---------------- EDIT POST ----------------

        [TestMethod]
        public async Task Edit_Post_RazlicitiIDevi_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var rezultat = await controller.Edit(
                2, KreirajDostavu(1));

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Post_ValidanModel_AzuriraDostavu()
        {
            using var context = KreirajContext();

            context.Dostave.Add(KreirajDostavu());
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var controller = new DostavasController(context);

            var izmijenjena = KreirajDostavu();
            izmijenjena.Adresa = "Zenica, Centar 5";
            izmijenjena.CijenaDostave = 8.0;

            var rezultat = await controller.Edit(1, izmijenjena);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            context.ChangeTracker.Clear();

            var sacuvana = await context.Dostave.FindAsync(1);
            Assert.IsNotNull(sacuvana);

            Assert.AreEqual("Zenica, Centar 5", sacuvana.Adresa);
            Assert.AreEqual(8.0, sacuvana.CijenaDostave);
        }

        [TestMethod]
        public async Task Edit_Post_NevalidanModel_VracaView()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var dostava = KreirajDostavu();

            controller.ModelState.AddModelError(
                "Adresa", "Nevalidna adresa");

            var rezultat = await controller.Edit(1, dostava);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            Assert.AreSame(dostava, view.Model);
        }

        [TestMethod]
        public async Task Edit_Post_NevalidanModel_PostavljaSelectList()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            controller.ModelState.AddModelError(
                "Adresa", "Nevalidna adresa");

            await controller.Edit(1, KreirajDostavu());

            Assert.IsInstanceOfType<SelectList>(
                controller.ViewData["NarudzbaID"]);
        }

        // ---------------- DELETE GET ----------------

        [TestMethod]
        public async Task Delete_Get_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var rezultat = await controller.Delete(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Delete_Get_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var rezultat = await controller.Delete(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        
[TestMethod]
public async Task Delete_Get_PostojeciID_VracaDostavu()
{
    using var context = KreirajContext();

    context.Narudzbe.Add(KreirajNarudzbu(1));
    context.Dostave.Add(KreirajDostavu(1));

    await context.SaveChangesAsync();

    var controller = new DostavasController(context);

    var rezultat = await controller.Delete(1);

    var view = rezultat as ViewResult;
    Assert.IsNotNull(view);

    var model = view.Model as Dostava;
    Assert.IsNotNull(model);

    Assert.AreEqual(1, model.DostavaID);
    Assert.IsNotNull(model.Narudzba);
    Assert.AreEqual(1, model.Narudzba.NarudzbaID);
}


        // ---------------- DELETE POST ----------------

        [TestMethod]
        public async Task DeleteConfirmed_PostojeciID_BriseDostavu()
        {
            using var context = KreirajContext();

            context.Dostave.Add(KreirajDostavu());
            await context.SaveChangesAsync();

            var controller = new DostavasController(context);

            var rezultat = await controller.DeleteConfirmed(1);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            Assert.AreEqual(0, await context.Dostave.CountAsync());
        }

        [TestMethod]
        public async Task DeleteConfirmed_NepostojeciID_VracaRedirect()
        {
            using var context = KreirajContext();
            var controller = new DostavasController(context);

            var rezultat = await controller.DeleteConfirmed(999);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            Assert.AreEqual(0, await context.Dostave.CountAsync());
        }

        // ---------------- CONCURRENCY ----------------

        [TestMethod]
        public async Task Edit_Post_KonfliktINepostojecaDostava_VracaNotFound()
        {
            var options = KreirajOpcije();

            using var context = new DostavaContextSaKonfliktom(options);
            var controller = new DostavasController(context);

            var rezultat = await controller.Edit(
                999, KreirajDostavu(999));

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Post_KonfliktIPostojecaDostava_BacaIzuzetak()
        {
            var options = KreirajOpcije();

            using (var context = new ApplicationDbContext(options))
            {
                context.Dostave.Add(KreirajDostavu());
                await context.SaveChangesAsync();
            }

            using var konfliktContext =
                new DostavaContextSaKonfliktom(options);

            var controller = new DostavasController(konfliktContext);

            await Assert.ThrowsExactlyAsync<DbUpdateConcurrencyException>(
                async () => await controller.Edit(
                    1, KreirajDostavu(1)));
        }
    }

    // Pomocna klasa za simulaciju konflikta u bazi.
    public class DostavaContextSaKonfliktom : ApplicationDbContext
    {
        public DostavaContextSaKonfliktom(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public override Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            throw new DbUpdateConcurrencyException(
                "Simulirani konflikt pri spremanju dostave.");
        }
    }
}
