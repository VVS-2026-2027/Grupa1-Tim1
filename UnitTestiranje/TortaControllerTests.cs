
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using BentoLab.Controllers;

namespace UnitTestiranje
{
    [TestClass]
    public class TortaControllerTests
    {
        private static TortaController KreirajController(
            Dictionary<string, object>? pocetniPodaci = null)
        {
            var httpContext = new DefaultHttpContext();

            var provider = new Mock<ITempDataProvider>();

            provider.Setup(p => p.LoadTempData(It.IsAny<HttpContext>()))
                .Returns(pocetniPodaci ?? new Dictionary<string, object>());

            var controller = new TortaController
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = httpContext
                },
                TempData = new TempDataDictionary(httpContext, provider.Object)
            };

            return controller;
        }

        // GET akcije

        [TestMethod]
        public void Index_VracaView()
        {
            var controller = KreirajController();

            var rezultat = controller.Index();

            Assert.IsInstanceOfType(rezultat, typeof(ViewResult));
        }

        [TestMethod]
        public void Cjenovnik_VracaView()
        {
            var controller = KreirajController();

            var rezultat = controller.Cjenovnik();

            Assert.IsInstanceOfType(rezultat, typeof(ViewResult));
        }

        [TestMethod]
        public void Galerija_VracaView()
        {
            var controller = KreirajController();

            var rezultat = controller.Galerija();

            Assert.IsInstanceOfType(rezultat, typeof(ViewResult));
        }

        [TestMethod]
        public void Kreiraj_VracaView_I_CuvaPostojecePodatke()
        {
            var podaci = new Dictionary<string, object>
            {
                ["NazivKreiraneTorte"] = new[] { "Čokoladna" },
                ["CijenaKreiraneTorte"] = new[] { "30.00" }
            };

            var controller = KreirajController(podaci);

            var rezultat = controller.Kreiraj();

            Assert.IsInstanceOfType(rezultat, typeof(ViewResult));
            Assert.IsNotNull(controller.TempData["NazivKreiraneTorte"]);
            Assert.IsNotNull(controller.TempData["CijenaKreiraneTorte"]);
        }

        [TestMethod]
        public void Dekoracija_GET_VracaView_I_CuvaIzbore()
        {
            var podaci = new Dictionary<string, object>
            {
                ["IzabraniOkus"] = "Nutella",
                ["IzabraniOblik"] = "Srce",
                ["IzabranaBoja"] = "Roza"
            };

            var controller = KreirajController(podaci);

            var rezultat = controller.Dekoracija();

            Assert.IsInstanceOfType(rezultat, typeof(ViewResult));
            Assert.AreEqual("Nutella", controller.TempData["IzabraniOkus"]);
            Assert.AreEqual("Srce", controller.TempData["IzabraniOblik"]);
            Assert.AreEqual("Roza", controller.TempData["IzabranaBoja"]);
        }

        [TestMethod]
        public void Procjena_VracaView()
        {
            var controller = KreirajController();

            var rezultat = controller.Procjena();

            Assert.IsInstanceOfType(rezultat, typeof(ViewResult));
        }

        // POST Kreiraj

        [TestMethod]
        public void Kreiraj_POST_SpremaIzbore_I_PreusmjeravaNaDekoraciju()
        {
            var controller = KreirajController();

            var rezultat = controller.Kreiraj("Nutella", "Srce", "Roza");

            Assert.IsInstanceOfType(rezultat, typeof(RedirectToActionResult));

            var redirect = (RedirectToActionResult)rezultat;

            Assert.AreEqual("Dekoracija", redirect.ActionName);
            Assert.AreEqual("Nutella", controller.TempData["IzabraniOkus"]);
            Assert.AreEqual("Srce", controller.TempData["IzabraniOblik"]);
            Assert.AreEqual("Roza", controller.TempData["IzabranaBoja"]);
        }



        [DataTestMethod]
        [DataRow("Bueno", "22.00")]
        [DataRow("Nutella", "22.00")]
        [DataRow("Raffaelo", "23.00")]
        [DataRow("Berry pistachio", "25.00")]
        [DataRow("Nepoznat okus", "20.00")]
        public void Dekoracija_POST_IspravnoRacunaOsnovnuCijenu(
            string okus, string ocekivanaCijena)
        {
            var podaci = new Dictionary<string, object>
            {
                ["IzabraniOkus"] = okus,
                ["IzabraniOblik"] = "Krug",
                ["IzabranaBoja"] = "Bijela"
            };

            var controller = KreirajController(podaci);

            var rezultat = controller.Dekoracija(
                new List<string>(), "Ništa", "Ništa od ponuđenog", "");

            Assert.IsInstanceOfType(rezultat, typeof(RedirectToActionResult));
            Assert.AreEqual(ocekivanaCijena, controller.TempData["OsnovnaCijena"]);
            Assert.AreEqual("0.00", controller.TempData["DodatnoCijena"]);
            Assert.AreEqual("1.25", controller.TempData["Koeficijent"]);
        }

        [TestMethod]
        public void Dekoracija_POST_RacunaSveDodatke()
        {
            var podaci = new Dictionary<string, object>
            {
                ["IzabraniOkus"] = "Bueno",
                ["IzabraniOblik"] = "Krug",
                ["IzabranaBoja"] = "Bijela"
            };

            var ukrasi = new List<string>
            {
                "Perlice",
                "Šljokice",
                "Cvijeće",
                "Mašnice",
                "Figurica"
            };

            var controller = KreirajController(podaci);

            var rezultat = controller.Dekoracija(
                ukrasi, "Čokoladni", "Voće", "Sretan rođendan");

            Assert.IsInstanceOfType(rezultat, typeof(RedirectToActionResult));

            Assert.AreEqual("14.00", controller.TempData["DodatnoCijena"]);
            Assert.AreEqual("22.00", controller.TempData["OsnovnaCijena"]);
            Assert.AreEqual("45.00", controller.TempData["UkupnaCijenaKonacna"]);
            Assert.AreEqual("Perlice, Šljokice, Cvijeće, Mašnice, Figurica",
                controller.TempData["IzabraniUkrasi"]);
            Assert.AreEqual("Čokoladni", controller.TempData["IzabraniSlag"]);
            Assert.AreEqual("Voće", controller.TempData["IzabraniDodaci"]);
            Assert.AreEqual("Sretan rođendan", controller.TempData["Napomena"]);
        }

        [TestMethod]
        public void Dekoracija_POST_BezUkrasaIDodataka_KoristiPodrazumijevaneVrijednosti()
        {
            var controller = KreirajController();

            var rezultat = controller.Dekoracija(
                null!, null!, null!, null!);

            Assert.IsInstanceOfType(rezultat, typeof(RedirectToActionResult));


            Assert.AreEqual("Nema ukrasa", controller.TempData["IzabraniUkrasi"]);
            Assert.AreEqual("Ništa", controller.TempData["IzabraniSlag"]);
            Assert.AreEqual("Ništa od ponuđenog", controller.TempData["IzabraniDodaci"]);
            Assert.AreEqual("22.00", controller.TempData["OsnovnaCijena"]);
Assert.AreEqual("27.50", controller.TempData["UkupnaCijenaKonacna"]);
        }

        [TestMethod]
        public void Dekoracija_POST_RacunaCijenuSaSlagomIDodatkom()
        {
            var podaci = new Dictionary<string, object>
            {
                ["IzabraniOkus"] = "Nepoznat okus"
            };

            var controller = KreirajController(podaci);

            controller.Dekoracija(
                new List<string>(), "Šlag", "Voće", "");

            Assert.AreEqual("2.00", controller.TempData["DodatnoCijena"]);
            Assert.AreEqual("27.50", controller.TempData["UkupnaCijenaKonacna"]);
        }

        [TestMethod]
        public void Dekoracija_POST_RacunaPojedinacneUkrase()
        {
            var controller = KreirajController();

            controller.Dekoracija(
                new List<string> { "Perlice", "Cvijeće", "Figurica" },
                "Ništa", "Ništa od ponuđenog", "");

            Assert.AreEqual("8.00", controller.TempData["DodatnoCijena"]);
Assert.AreEqual("37.50", controller.TempData["UkupnaCijenaKonacna"]);        }

        

        [TestMethod]
        public void DodajUKorpu_DodajePrvuTortu_I_PreusmjeravaNaKorpu()
        {
            var controller = KreirajController();

            var rezultat = controller.DodajUKorpu("Čokoladna", "30.5");

            Assert.IsInstanceOfType(rezultat, typeof(RedirectToActionResult));

            var redirect = (RedirectToActionResult)rezultat;

            Assert.AreEqual("Index", redirect.ActionName);
            Assert.AreEqual("Korpas", redirect.ControllerName);

            var nazivi = controller.TempData["NazivKreiraneTorte"] as string[];
            var cijene = controller.TempData["CijenaKreiraneTorte"] as string[];

            CollectionAssert.AreEqual(new[] { "Čokoladna" }, nazivi);
            CollectionAssert.AreEqual(new[] { "30.50" }, cijene);
        }

        [TestMethod]
        public void DodajUKorpu_DodajeTortuNaPostojeceTorte()
        {
            var podaci = new Dictionary<string, object>
            {
                ["NazivKreiraneTorte"] = new[] { "Čokoladna" },
                ["CijenaKreiraneTorte"] = new[] { "30.00" }
            };

            var controller = KreirajController(podaci);

            controller.DodajUKorpu("Voćna", "25");

            var nazivi = controller.TempData["NazivKreiraneTorte"] as string[];
            var cijene = controller.TempData["CijenaKreiraneTorte"] as string[];

            CollectionAssert.AreEqual(
                new[] { "Čokoladna", "Voćna" }, nazivi);

            CollectionAssert.AreEqual(
                new[] { "30.00", "25.00" }, cijene);
        }

        [TestMethod]
        public void DodajUKorpu_CijenuFormatiraNaDvijeDecimale()
        {
            var controller = KreirajController();

            controller.DodajUKorpu("Voćna", "19.999");

            var cijene = controller.TempData["CijenaKreiraneTorte"] as string[];

            CollectionAssert.AreEqual(new[] { "20.00" }, cijene);
        }

       [TestMethod]
public void DodajUKorpu_NeispravnaCijena_BacaFormatException()
{
    var controller = KreirajController();

    Assert.Throws<FormatException>(() =>
        controller.DodajUKorpu("Čokoladna", "nije cijena"));
}


[TestMethod]
public void Kreiraj_KoristiMockTempDataProvider()
{
    var httpContext = new DefaultHttpContext();

    var provider = new Mock<ITempDataProvider>();

    provider.Setup(p => p.LoadTempData(It.IsAny<HttpContext>()))
        .Returns(new Dictionary<string, object>
        {
            ["NazivKreiraneTorte"] = new[] { "Čokoladna" },
            ["CijenaKreiraneTorte"] = new[] { "30.00" }
        });

    var controller = new TortaController
    {
        ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        },
        TempData = new TempDataDictionary(httpContext, provider.Object)
    };

    var rezultat = controller.Kreiraj();

    Assert.IsInstanceOfType(rezultat, typeof(ViewResult));

    provider.Verify(
        p => p.LoadTempData(It.IsAny<HttpContext>()),
        Times.Once
    );
}

    }
}
