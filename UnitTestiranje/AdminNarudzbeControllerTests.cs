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
using System.IO;
using System.Text.Json;

namespace UnitTestiranje
{
    [TestClass]
    public class AdminNarudzbeControllerTests
    {
        private ApplicationDbContext KreirajContext()
        {
            var options =
                new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDbContext(options);
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
            var controller = new AdminNarudzbeController(context);

            var rezultat = await controller.Index();

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as List<Narudzba>;
            Assert.IsNotNull(model);
            Assert.HasCount(0, model);
        }

        [TestMethod]
        public async Task Index_PostojeciPodaci_VracaSveNarudzbe()
        {
            using var context = KreirajContext();

            context.Narudzbe.AddRange(
                KreirajNarudzbu(1),
                KreirajNarudzbu(2),
                KreirajNarudzbu(3));

            await context.SaveChangesAsync();

            var controller = new AdminNarudzbeController(context);

            var rezultat = await controller.Index();

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as List<Narudzba>;
            Assert.IsNotNull(model);
            Assert.HasCount(3, model);
        }

        [TestMethod]
        public async Task Index_NarudzbeNisuSortirane_VracaSortiranePoIDu()
        {
            using var context = KreirajContext();

            context.Narudzbe.AddRange(
                KreirajNarudzbu(5),
                KreirajNarudzbu(2),
                KreirajNarudzbu(8),
                KreirajNarudzbu(1));

            await context.SaveChangesAsync();

            var controller = new AdminNarudzbeController(context);

            var rezultat = await controller.Index();

            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as List<Narudzba>;
            Assert.IsNotNull(model);

            CollectionAssert.AreEqual(
                new List<int> { 1, 2, 5, 8 },
                model.Select(n => n.NarudzbaID).ToList());
        }

        // ---------------- DETAILS ----------------

        [TestMethod]
        public async Task Details_NullID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new AdminNarudzbeController(context);

            var rezultat = await controller.Details(null);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]
        public async Task Details_NepostojeciID_VracaNotFound()
        {
            using var context = KreirajContext();
            var controller = new AdminNarudzbeController(context);

            var rezultat = await controller.Details(999);

            Assert.IsInstanceOfType<NotFoundResult>(rezultat);
        }

        [TestMethod]

        public async Task Details_PostojeciID_VracaNarudzbu()
        {
            // Arrange
            using var context = KreirajContext();

            // Kreiramo korisnika koji ima ID = 1
            var korisnik = new Korisnik
            {
                Id = 1,
                UserName = "testkorisnik",
                Email = "test@example.com",
                ImePrezime = "Test Korisnik",
                Uloga = Uloga.KUPAC
            };

            context.Users.Add(korisnik);

            // Kreiramo narudzbu povezanu s korisnikom
            var narudzba = KreirajNarudzbu(1);
            narudzba.KorisnikID = korisnik.Id;
            narudzba.Korisnik = korisnik;

            context.Narudzbe.Add(narudzba);

            await context.SaveChangesAsync();

            // Act
            var controller = new AdminNarudzbeController(context);

            var rezultat = await controller.Details(1);

            // Assert
            var view = rezultat as ViewResult;
            Assert.IsNotNull(view);

            var model = view.Model as Narudzba;
            Assert.IsNotNull(model);

            Assert.AreEqual(1, model.NarudzbaID);
            Assert.AreEqual(50.0, model.UkupnaCijena);

            // Provjeravamo povezivanje s korisnikom
            Assert.IsNotNull(model.Korisnik);
            Assert.AreEqual(1, model.Korisnik.Id);
            Assert.AreEqual("Test Korisnik", model.Korisnik.ImePrezime);
        }


        // ---------------- PROMIJENI STATUS ----------------

        [TestMethod]
        public async Task PromijeniStatus_NepostojecaNarudzba_VracaRedirect()
        {
            using var context = KreirajContext();
            var controller = new AdminNarudzbeController(context);

            var rezultat = await controller.PromijeniStatus(
                999, "POTVRDJENA");

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);
        }

        [TestMethod]
        public async Task PromijeniStatus_ValidanStatus_AzuriraNarudzbu()
        {
            using var context = KreirajContext();

            context.Narudzbe.Add(KreirajNarudzbu(1));
            await context.SaveChangesAsync();

            var controller = new AdminNarudzbeController(context);

            var noviStatus = StatusNarudzbe.POTVRDJENA.ToString();

            var rezultat = await controller.PromijeniStatus(
                1, noviStatus);

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            context.ChangeTracker.Clear();

            var narudzba = await context.Narudzbe.FindAsync(1);
            Assert.IsNotNull(narudzba);

            Assert.AreEqual(
                StatusNarudzbe.POTVRDJENA,
                narudzba.Status);
        }

        [TestMethod]
        public async Task PromijeniStatus_NevalidanStatus_NeMijenjaNarudzbu()
        {
            using var context = KreirajContext();

            context.Narudzbe.Add(KreirajNarudzbu(1));
            await context.SaveChangesAsync();

            var controller = new AdminNarudzbeController(context);

            var rezultat = await controller.PromijeniStatus(
                1, "NEPOSTOJECI_STATUS");

            var redirect = rezultat as RedirectToActionResult;
            Assert.IsNotNull(redirect);
            Assert.AreEqual("Index", redirect.ActionName);

            context.ChangeTracker.Clear();

            var narudzba = await context.Narudzbe.FindAsync(1);
            Assert.IsNotNull(narudzba);

            Assert.AreEqual(
                StatusNarudzbe.KREIRANA,
                narudzba.Status);
        }

        [TestMethod]
        public async Task PromijeniStatus_PrazanStatus_NeMijenjaNarudzbu()
        {
            using var context = KreirajContext();

            context.Narudzbe.Add(KreirajNarudzbu(1));
            await context.SaveChangesAsync();

            var controller = new AdminNarudzbeController(context);

            var rezultat = await controller.PromijeniStatus(1, "");

            Assert.IsInstanceOfType<RedirectToActionResult>(rezultat);

            context.ChangeTracker.Clear();

            var narudzba = await context.Narudzbe.FindAsync(1);
            Assert.IsNotNull(narudzba);

            Assert.AreEqual(
                StatusNarudzbe.KREIRANA,
                narudzba.Status);
        }

        [TestMethod]
        public async Task PromijeniStatus_IstiStatus_OstajeNepromijenjen()
        {
            using var context = KreirajContext();

            context.Narudzbe.Add(KreirajNarudzbu(1));
            await context.SaveChangesAsync();

            var controller = new AdminNarudzbeController(context);

            var rezultat = await controller.PromijeniStatus(
                1, StatusNarudzbe.KREIRANA.ToString());

            Assert.IsInstanceOfType<RedirectToActionResult>(rezultat);

            context.ChangeTracker.Clear();

            var narudzba = await context.Narudzbe.FindAsync(1);
            Assert.IsNotNull(narudzba);

            Assert.AreEqual(
                StatusNarudzbe.KREIRANA,
                narudzba.Status);
        }

        
[DataTestMethod]
[DynamicData(nameof(CSVPodaci), DynamicDataSourceType.Method)]
public async Task PromijeniStatus_CSV_PodaciIzDatoteke(
    string noviStatus, string ocekivaniStatus)
{
    await ProvjeriPromjenuStatusa(noviStatus, ocekivaniStatus);
}

[DataTestMethod]
[DynamicData(nameof(JSONPodaci), DynamicDataSourceType.Method)]
public async Task PromijeniStatus_JSON_PodaciIzDatoteke(
    string noviStatus, string ocekivaniStatus)
{
    await ProvjeriPromjenuStatusa(noviStatus, ocekivaniStatus);
}

private async Task ProvjeriPromjenuStatusa(
    string noviStatus, string ocekivaniStatus)
{
    using var context = KreirajContext();

    var narudzba = KreirajNarudzbu(1);
    context.Narudzbe.Add(narudzba);
    await context.SaveChangesAsync();

    var controller = new AdminNarudzbeController(context);

    var rezultat = await controller.PromijeniStatus(1, noviStatus);

    Assert.IsInstanceOfType<RedirectToActionResult>(rezultat);

    var sacuvana = await context.Narudzbe.FindAsync(1);

    Assert.IsNotNull(sacuvana);
    Assert.AreEqual(ocekivaniStatus, sacuvana.Status.ToString());
}


        // Pomocna klasa za JSON podatke
        public class StatusTestPodatak
        {
            public string NoviStatus { get; set; } = "";
            public string OcekivaniStatus { get; set; } = "";
        }


public static IEnumerable<object[]> CSVPodaci()
{
    string putanja = Path.Combine(
        AppContext.BaseDirectory, "TestData", "statusi.csv");

    return File.ReadAllLines(putanja)
        .Skip(1)
        .Where(red => !string.IsNullOrWhiteSpace(red))
        .Select(red =>
        {
            var dijelovi = red.Split(',');
            return new object[]
            {
                dijelovi[0].Trim(),
                dijelovi[1].Trim()
            };
        });
}

public static IEnumerable<object[]> JSONPodaci()
{
    string putanja = Path.Combine(
        AppContext.BaseDirectory, "TestData", "statusi.json");

    string json = File.ReadAllText(putanja);

    var podaci = JsonSerializer.Deserialize<List<StatusTestPodatak>>(json)
        ?? throw new InvalidDataException("JSON nema testnih podataka.");

    return podaci.Select(p => new object[]
    {
        p.NoviStatus,
        p.OcekivaniStatus
    });
}


    }
}
