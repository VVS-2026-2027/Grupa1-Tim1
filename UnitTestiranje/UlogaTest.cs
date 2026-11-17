
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BentoLab.Models;

namespace UnitTestiranje
{
    [TestClass]
    public class UlogaTests
    {
        [TestMethod]
        public void Uloga_SadrziDvijeVrijednosti()
        {
            var vrijednosti = Enum.GetValues<Uloga>();

            Assert.AreEqual(2, vrijednosti.Length);
        }

        [TestMethod]
        public void Uloga_ImaIspravneNumerickeVrijednosti()
        {
            Assert.AreEqual(0, (int)Uloga.KUPAC);
            Assert.AreEqual(1, (int)Uloga.ADMIN);
        }

        [TestMethod]
        public void Uloga_SadrziOcekivaneNazive()
        {
            CollectionAssert.AreEqual(
                new[] { "KUPAC", "ADMIN" },
                Enum.GetNames<Uloga>()
            );
        }
    }
}
