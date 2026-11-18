using BentoLab.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestiranje
{
    [TestClass]
    public class StavkaNarudzbeTests
    {
        [TestMethod]
        public void Konstruktor_NovaStavka_VrijednostiSuPodrazumijevane()
        {
            var stavka = new StavkaNarudzbe();

            Assert.AreEqual(0, stavka.StavkaID);
            Assert.AreEqual(0, stavka.Kolicina);
            Assert.AreEqual(0.0, stavka.CijenaStavke);
            Assert.IsNull(stavka.Narudzba);
            Assert.IsNull(stavka.Torta);
        }

        [TestMethod]
        public void Svojstva_PostavljenaVrijednost_Cuva()
        {
            var stavka = new StavkaNarudzbe
            {
                StavkaID = 5,
                Kolicina = 3,
                CijenaStavke = 45.5,
                NarudzbaID = 2,
                TortaID = 7
            };

            Assert.AreEqual(5, stavka.StavkaID);
            Assert.AreEqual(3, stavka.Kolicina);
            Assert.AreEqual(45.5, stavka.CijenaStavke);
            Assert.AreEqual(2, stavka.NarudzbaID);
            Assert.AreEqual(7, stavka.TortaID);
        }
    }
}
