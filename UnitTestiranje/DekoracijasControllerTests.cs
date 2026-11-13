using BentoLab.Controllers;
using BentoLab.Data;
using BentoLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UnitTestiranje
{
    [TestClass]
    public class DekoracijasControllerTests
    {
        // Za svaki test kreiramo nezavisnu InMemory bazu.
        private ApplicationDbContext KreirajContext()
        {
            var options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
        }

        // Pomocna metoda za kreiranje dekoracije.
        private Dekoracija KreirajDekoraciju(int id = 1)
        {
            return new Dekoracija
            {
                DekoracijaID = id,
                Naziv = "Cvjetna dekoracija",
                Cijena = 15.50,
                TipDekoracije = TipDekoracije.JESTIVO_CVIJECE
            };
        }

        // ---------------- INDEX ----------------

        [TestMethod]
        public async Task Index_PraznaBaza_VracaPraznuListu()
        {
            // Arrange
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);

            // Act
            var rezultat = await controller.Index();

            // Assert
            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as List<Dekoracija>;
            Assert.IsNotNull(model);
            Assert.AreEqual(0, model.Count);
        }

        [TestMethod]
        public async Task Index_PostojeciPodaci_VracaSveDekoracije()
        {
            // Arrange
            using var context = KreirajContext();

            context.Dekoracija.Add(KreirajDekoraciju(1));
            context.Dekoracija.Add(KreirajDekoraciju(2));
            await context.SaveChangesAsync();

            var controller = new DekoracijasController(context);

            // Act
            var rezultat = await controller.Index();

            // Assert
            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as List<Dekoracija>;
            Assert.IsNotNull(model);
            Assert.AreEqual(2, model.Count);
        }

        // ---------------- DETAILS ----------------

        [TestMethod]
        public async Task Details_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);

            var rezultat = await controller.Details(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Details_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);

            var rezultat = await controller.Details(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Details_PostojeciID_VracaDekoraciju()
        {
            using var context = KreirajContext();

            context.Dekoracija.Add(KreirajDekoraciju());
            await context.SaveChangesAsync();

            var controller = new DekoracijasController(context);

            var rezultat = await controller.Details(1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as Dekoracija;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.DekoracijaID);
        }

        // ---------------- CREATE GET ----------------

        [TestMethod]
        public void Create_Get_VracaView()
        {
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);

            var rezultat = controller.Create();

            Assert.IsInstanceOfType<ViewResult>(rezultat);
        }

        // ---------------- CREATE POST ----------------

        [TestMethod]
        public async Task Create_ValidanModel_DodajeDekoraciju()
        {
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);
            var dekoracija = KreirajDekoraciju();

            var rezultat = await controller.Create(dekoracija);

            Assert.IsInstanceOfType<RedirectToActionResult>(rezultat);

            var redirect = (RedirectToActionResult)rezultat;
            Assert.AreEqual("Index", redirect.ActionName);

            Assert.AreEqual(1, await context.Dekoracija.CountAsync());
        }

        [TestMethod]
        public async Task Create_NevalidanModel_VracaViewBezSpremanja()
        {
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);
            var dekoracija = KreirajDekoraciju();

            controller.ModelState.AddModelError(
                "Naziv", "Naziv nije validan");

            var rezultat = await controller.Create(dekoracija);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            Assert.AreSame(dekoracija, view.Model);

            Assert.AreEqual(0, await context.Dekoracija.CountAsync());
        }

        // ---------------- EDIT GET ----------------

        [TestMethod]
        public async Task Edit_Get_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);

            var rezultat = await controller.Edit((int?)null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Get_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);

            var rezultat = await controller.Edit((int?)999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Get_PostojeciID_VracaDekoraciju()
        {
            using var context = KreirajContext();

            context.Dekoracija.Add(KreirajDekoraciju());
            await context.SaveChangesAsync();

            var controller = new DekoracijasController(context);

            var rezultat = await controller.Edit((int?)1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as Dekoracija;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.DekoracijaID);
        }

        // ---------------- EDIT POST ----------------

        [TestMethod]
        public async Task Edit_Post_RazlicitiIDevi_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);
            var dekoracija = KreirajDekoraciju(1);

            var rezultat = await controller.Edit(2, dekoracija);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Edit_Post_ValidanModel_AzuriraDekoraciju()
        {
            using var context = KreirajContext();

            context.Dekoracija.Add(KreirajDekoraciju());
            await context.SaveChangesAsync();

            context.ChangeTracker.Clear();

            var controller = new DekoracijasController(context);

            var izmijenjena = KreirajDekoraciju();
            izmijenjena.Naziv = "Nova dekoracija";
            izmijenjena.Cijena = 25.0;

            var rezultat = await controller.Edit(1, izmijenjena);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            context.ChangeTracker.Clear();

            var sacuvana = await context.Dekoracija.FindAsync(1);

            Assert.IsNotNull(sacuvana);
            Assert.AreEqual("Nova dekoracija", sacuvana.Naziv);
            Assert.AreEqual(25.0, sacuvana.Cijena);
        }

        [TestMethod]
        public async Task Edit_Post_NevalidanModel_VracaView()
        {
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);
            var dekoracija = KreirajDekoraciju();

            controller.ModelState.AddModelError(
                "Naziv", "Nevalidan naziv");

            var rezultat = await controller.Edit(1, dekoracija);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);
            Assert.AreSame(dekoracija, view.Model);
        }

        // ---------------- DELETE GET ----------------

        [TestMethod]
        public async Task Delete_Get_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);

            var rezultat = await controller.Delete(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Delete_Get_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);

            var rezultat = await controller.Delete(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Delete_Get_PostojeciID_VracaDekoraciju()
        {
            using var context = KreirajContext();

            context.Dekoracija.Add(KreirajDekoraciju());
            await context.SaveChangesAsync();

            var controller = new DekoracijasController(context);

            var rezultat = await controller.Delete(1);

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as Dekoracija;
            Assert.IsNotNull(model);
            Assert.AreEqual(1, model.DekoracijaID);
        }

        // ---------------- DELETE POST ----------------

        [TestMethod]
        public async Task DeleteConfirmed_PostojeciID_BriseDekoraciju()
        {
            using var context = KreirajContext();

            context.Dekoracija.Add(KreirajDekoraciju());
            await context.SaveChangesAsync();

            var controller = new DekoracijasController(context);

            var rezultat = await controller.DeleteConfirmed(1);

            Assert.IsInstanceOfType<RedirectToActionResult>(rezultat);
            Assert.AreEqual(0, await context.Dekoracija.CountAsync());
        }

        [TestMethod]
        public async Task DeleteConfirmed_NepostojeciID_NeBacaIzuzetak()
        {
            using var context = KreirajContext();
            var controller = new DekoracijasController(context);

            var rezultat = await controller.DeleteConfirmed(999);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            Assert.AreEqual(0, await context.Dekoracija.CountAsync());
        }

[TestMethod]
public async Task Edit_Post_KonfliktINepostojecaDekoracija_VracaNotFound()
{
    var options =
        new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

    using var context = new ContextSaKonfliktom(options);
    var controller = new DekoracijasController(context);

    var dekoracija = KreirajDekoraciju(999);

    var rezultat = await controller.Edit(999, dekoracija);

    Assert.IsInstanceOfType<NotFoundResult>(rezultat);
}

[TestMethod]
public async Task Edit_Post_KonfliktIPostojecaDekoracija_BacaIzuzetak()
{
    var options =
        new DbContextOptionsBuilder<ApplicationDbContext>()
        .UseInMemoryDatabase(Guid.NewGuid().ToString())
        .Options;

    // Prvo spremamo dekoraciju obicnim kontekstom.
    using (var context = new ApplicationDbContext(options))
    {
        context.Dekoracija.Add(KreirajDekoraciju(1));
        await context.SaveChangesAsync();
    }

    // Zatim koristimo kontekst koji simulira konflikt.
    using var konfliktContext = new ContextSaKonfliktom(options);
    var controller = new DekoracijasController(konfliktContext);

    var dekoracija = KreirajDekoraciju(1);
    dekoracija.Naziv = "Izmijenjena dekoracija";

    await Assert.ThrowsExactlyAsync<DbUpdateConcurrencyException>(
        async () => await controller.Edit(1, dekoracija));
}

    }

public class ContextSaKonfliktom : ApplicationDbContext
{
    public ContextSaKonfliktom(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public override Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        throw new DbUpdateConcurrencyException(
            "Simulirani konflikt pri spremanju.");
    }
}

}
