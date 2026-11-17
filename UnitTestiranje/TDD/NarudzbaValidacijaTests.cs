using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BentoLab.Models;
using BentoLab.Services;

namespace UnitTestiranje
{
    [TestClass]
    public class NarudzbaValidacijaTests
    {
        private NarudzbaServis _servis;

        [TestInitialize]
        public void Setup()
        {
            _servis = new NarudzbaServis();
        }

        [TestMethod]
        public void ValidirajOtkazivanje_StatusKreirana_NeBacaIzuzetak()
        {
            var narudzba = new Narudzba
            {
                NarudzbaID = 1,
                Status = StatusNarudzbe.KREIRANA
            };

            _servis.ValidirajOtkazivanje(narudzba);
        }

        [TestMethod]
        public void ValidirajOtkazivanje_NarudzbaNePostoji_BacaArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() =>
                _servis.ValidirajOtkazivanje(null));
        }

        [TestMethod]
        public void ValidirajOtkazivanje_VecOtkazana_BacaInvalidOperationException()
        {
            var narudzba = new Narudzba
            {
                NarudzbaID = 2,
                Status = StatusNarudzbe.OTKAZANA
            };

            Assert.Throws<InvalidOperationException>(() =>
                _servis.ValidirajOtkazivanje(narudzba));
        }

        [TestMethod]
        public void ValidirajOtkazivanje_StatusPotvrdjena_BacaInvalidOperationException()
        {
            var narudzba = new Narudzba
            {
                NarudzbaID = 3,
                Status = StatusNarudzbe.POTVRDJENA
            };

            Assert.Throws<InvalidOperationException>(() =>
                _servis.ValidirajOtkazivanje(narudzba));
        }
    }
}
