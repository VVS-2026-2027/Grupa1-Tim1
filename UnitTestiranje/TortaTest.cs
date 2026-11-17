
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BentoLab.Models;

namespace UnitTestiranje
{
    [TestClass]
    public class TortaTests
    {
        [TestMethod]
        public void Torta_KonstruktorKreiraObjekat()
        {
            var torta = new Torta();

            Assert.IsNotNull(torta);
        }

        [TestMethod]
        public void Torta_PostavljaICitaTortaID()
        {
            var torta = new Torta
            {
                TortaID = 10
            };

            Assert.AreEqual(10, torta.TortaID);
        }

        [TestMethod]
        public void Torta_PostavljaICitaNaziv()
        {
            var torta = new Torta
            {
                Naziv = "Čokoladna torta"
            };

            Assert.AreEqual("Čokoladna torta", torta.Naziv);
        }

        [TestMethod]
        public void Torta_PostavljaICitaCijenu()
        {
            var torta = new Torta
            {
                Cijena = 35.50
            };

            Assert.AreEqual(35.50, torta.Cijena);
        }

        [TestMethod]
        public void Torta_PostavljaICitaKolicinuNaStanju()
        {
            var torta = new Torta
            {
                KolicinaNaStanju = 15
            };

            Assert.AreEqual(15, torta.KolicinaNaStanju);
        }

        [TestMethod]
        public void Torta_PostavljaICitaDostupnost()
        {
            var torta = new Torta
            {
                Dostupna = true
            };

            Assert.IsTrue(torta.Dostupna);

            torta.Dostupna = false;

            Assert.IsFalse(torta.Dostupna);
        }
    }
}
