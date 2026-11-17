using Microsoft.VisualStudio.TestTools.UnitTesting;
using BentoLab.Services;
using System;

namespace UnitTestiranje
{
    [TestClass]
    public class PopustCalculatorTests
    {
        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(30, 0)]
        [DataRow(49.99, 0)]
        [DataRow(50, 5)]
        [DataRow(75, 5)]
        [DataRow(99.99, 5)]
        [DataRow(100, 10)]
        [DataRow(150, 10)]
        public void IzracunajProcenat_IspravanIznos_VracaPopust(
            double iznos, double ocekivaniPopust)
        {
            var calculator = new PopustCalculator();

            var rezultat = calculator.IzracunajProcenat(
                (decimal)iznos);

            Assert.AreEqual(
                (decimal)ocekivaniPopust,
                rezultat);
        }

        [TestMethod]
        public void IzracunajProcenat_NegativanIznos_BacaIzuzetak()
        {
            var calculator = new PopustCalculator();

            Assert.ThrowsExactly<ArgumentOutOfRangeException>(
                () => calculator.IzracunajProcenat(-10m));
        }
    }
}
