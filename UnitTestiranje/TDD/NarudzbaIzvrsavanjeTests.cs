using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BentoLab.Controllers;
using BentoLab.Data;
using BentoLab.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestiranje
{
    [TestClass]
    public class NarudzbaIzvrsavanjeTests
    {
        [TestMethod]
        public async Task OtkaziNarudzbu_KreiranaNarudzba_StatusPostajeOtkazanaUBazi()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Narudzbe.Add(new Narudzba
                {
                    NarudzbaID = 100,
                    KorisnikID = 1,
                    Status = StatusNarudzbe.KREIRANA
                });
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var controller = new AdminNarudzbeController(context);
                controller.TempData = new TempDataDictionary(
                    new DefaultHttpContext(), new LazniTempDataProvider());
                await controller.OtkaziNarudzbu(100);
            }

            using (var context = new ApplicationDbContext(options))
            {
                var narudzba = await context.Narudzbe.FindAsync(100);
                Assert.AreEqual(StatusNarudzbe.OTKAZANA, narudzba!.Status);
            }
        }

        [TestMethod]
        public async Task OtkaziNarudzbu_KreiranaNarudzba_PostavljaPorukuUTempData()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            context.Narudzbe.Add(new Narudzba
            {
                NarudzbaID = 100,
                KorisnikID = 1,
                Status = StatusNarudzbe.KREIRANA
            });
            await context.SaveChangesAsync();

            var controller = new AdminNarudzbeController(context);
            controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(), new LazniTempDataProvider());

            await controller.OtkaziNarudzbu(100);

            Assert.AreEqual("Narudžba je uspješno otkazana.", controller.TempData["Poruka"]);
        }

        [TestMethod]
        public async Task OtkaziNarudzbu_PotvrdjenaNarudzba_StatusSeNeMijenjaUBazi()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Narudzbe.Add(new Narudzba
                {
                    NarudzbaID = 100,
                    KorisnikID = 1,
                    Status = StatusNarudzbe.POTVRDJENA
                });
                await context.SaveChangesAsync();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var controller = new AdminNarudzbeController(context);
                controller.TempData = new TempDataDictionary(
                    new DefaultHttpContext(), new LazniTempDataProvider());
                await controller.OtkaziNarudzbu(100);
            }

            using (var context = new ApplicationDbContext(options))
            {
                var narudzba = await context.Narudzbe.FindAsync(100);
                Assert.AreEqual(StatusNarudzbe.POTVRDJENA, narudzba!.Status);
            }
        }

        [TestMethod]
        public async Task OtkaziNarudzbu_PotvrdjenaNarudzba_PostavljaPorukuGreske()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);
            context.Narudzbe.Add(new Narudzba
            {
                NarudzbaID = 100,
                KorisnikID = 1,
                Status = StatusNarudzbe.POTVRDJENA
            });
            await context.SaveChangesAsync();

            var controller = new AdminNarudzbeController(context);
            controller.TempData = new TempDataDictionary(
                new DefaultHttpContext(), new LazniTempDataProvider());

            await controller.OtkaziNarudzbu(100);

            Assert.AreEqual("Narudžba se ne može otkazati.", controller.TempData["Greska"]);
        }
    }

    public class LazniTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context)
        {
            return new Dictionary<string, object>();
        }

        public void SaveTempData(HttpContext context, IDictionary<string, object> values)
        {
        }
    }
}