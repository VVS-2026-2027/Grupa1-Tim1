using BentoLab.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestiranje
{
    [TestClass]
    public class ObavjestenjeTests
    {
        [TestMethod]
        public void Konstruktor_NovoObavjestenje_VrijednostiSuPodrazumijevane()
        {
            var obavjestenje = new Obavjestenje();

            Assert.AreEqual(0, obavjestenje.ObavjestenjeID);
            Assert.IsNull(obavjestenje.Sadrzaj);
            Assert.AreEqual(default(DateTime), obavjestenje.DatumSlanja);
            Assert.AreEqual(0, obavjestenje.KorisnikID);
            Assert.AreEqual(0, obavjestenje.NarudzbaID);
            Assert.IsNull(obavjestenje.Korisnik);
            Assert.IsNull(obavjestenje.Narudzba);
        }

        [TestMethod]
        public void Svojstva_PostavljenaVrijednost_Cuva()
        {
            var datum = new DateTime(2026, 11, 17);

            var obavjestenje = new Obavjestenje
            {
                ObavjestenjeID = 4,
                Sadrzaj = "Narudzba je potvrdjena",
                DatumSlanja = datum,
                KorisnikID = 1,
                NarudzbaID = 9
            };

            Assert.AreEqual(4, obavjestenje.ObavjestenjeID);
            Assert.AreEqual("Narudzba je potvrdjena", obavjestenje.Sadrzaj);
            Assert.AreEqual(datum, obavjestenje.DatumSlanja);
            Assert.AreEqual(1, obavjestenje.KorisnikID);
            Assert.AreEqual(9, obavjestenje.NarudzbaID);
        }

        [TestMethod]
        public void Veze_PostavljenKorisnikINarudzba_CuvaReference()
        {
            var korisnik = new Korisnik { Id = 1 };
            var narudzba = new Narudzba { NarudzbaID = 9 };

            var obavjestenje = new Obavjestenje { Korisnik = korisnik, Narudzba = narudzba };

            Assert.AreSame(korisnik, obavjestenje.Korisnik);
            Assert.AreSame(narudzba, obavjestenje.Narudzba);
        }
    }
}