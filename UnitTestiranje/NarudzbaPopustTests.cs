using BentoLab.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestiranje
{
    [TestClass]
    public class NarudzbaPopustTests
    {
        [TestMethod]
        public void PrimijeniPopust_ValidanPopust_SmanjujeUkupnuCijenu()
        {
            var narudzba = new Narudzba { UkupnaCijena = 80.00 };

            var konacnaCijena = narudzba.PrimijeniPopust(4.00);

            Assert.AreEqual(76.00, konacnaCijena, 0.001);
            Assert.AreEqual(76.00, narudzba.UkupnaCijena, 0.001);
        }

        [TestMethod]
        public void PrimijeniPopust_NultiPopust_CijenaOstajeIsta()
        {
            var narudzba = new Narudzba { UkupnaCijena = 45.00 };

            var konacnaCijena = narudzba.PrimijeniPopust(0.00);

            Assert.AreEqual(45.00, konacnaCijena, 0.001);
        }

        [TestMethod]
        public void PrimijeniPopust_PopustVeciOdUkupneCijene_NePostajeNegativna()
        {
            var narudzba = new Narudzba { UkupnaCijena = 30.00 };

            var konacnaCijena = narudzba.PrimijeniPopust(50.00);

            Assert.AreEqual(0.00, konacnaCijena, 0.001);
            Assert.AreEqual(0.00, narudzba.UkupnaCijena, 0.001);
        }

        [DataTestMethod]
        [DataRow(55.55, 2.7775, 52.77)]
        [DataRow(99.99, 4.9995, 94.99)]
        [DataRow(123.45, 12.345, 111.10)]
        public void PrimijeniPopust_DecimalniIznosi_PravilnoZaokruzujeNaDvijeDecimale(
            double pocetnaCijena, double popust, double ocekivanaCijena)
        {
            var narudzba = new Narudzba { UkupnaCijena = pocetnaCijena };

            var konacnaCijena = narudzba.PrimijeniPopust(popust);

            Assert.AreEqual(ocekivanaCijena, konacnaCijena, 0.001);
        }

        [TestMethod]
        public void PrimijeniPopust_NegativanPopust_BacaArgumentException()
        {
            var narudzba = new Narudzba { UkupnaCijena = 60.00 };

            Assert.ThrowsExactly<ArgumentException>(() => narudzba.PrimijeniPopust(-5.00));
        }
    }
}
