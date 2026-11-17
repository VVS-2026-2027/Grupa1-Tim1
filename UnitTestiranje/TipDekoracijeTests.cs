
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BentoLab.Models;

namespace UnitTestiranje
{
    [TestClass]
    public class TipDekoracijeTests
    {
        [TestMethod]
        public void TipDekoracije_SadrziSvihPetVrijednosti()
        {
            var vrijednosti = Enum.GetValues<TipDekoracije>();

            Assert.AreEqual(5, vrijednosti.Length);
        }

        [TestMethod]
        public void TipDekoracije_ImaIspravneVrijednosti()
        {
            Assert.AreEqual(0, (int)TipDekoracije.JESTIVE_SLJOKICE);
            Assert.AreEqual(1, (int)TipDekoracije.JESTIVE_PERLICE);
            Assert.AreEqual(2, (int)TipDekoracije.MASNICE);
            Assert.AreEqual(3, (int)TipDekoracije.JESTIVO_CVIJECE);
            Assert.AreEqual(4, (int)TipDekoracije.COKOLADNA_FIGURICA);
        }

        [TestMethod]
        public void TipDekoracije_SadrziOcekivaneNazive()
        {
            CollectionAssert.AreEqual(
                new[]
                {
                    "JESTIVE_SLJOKICE",
                    "JESTIVE_PERLICE",
                    "MASNICE",
                    "JESTIVO_CVIJECE",
                    "COKOLADNA_FIGURICA"
                },
                Enum.GetNames<TipDekoracije>()
            );
        }
    }
}
