using BentoLab.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace UnitTestiranje
{
    [TestClass]
    public class NacinPreuzimanjaTests
    {
        [TestMethod]
        public void NacinPreuzimanja_BrojDefinisanihVrijednosti_TacnoDvijeOpcije()
        {
            // Act
            var vrijednosti = Enum.GetValues<NacinPreuzimanja>();

            // Assert
            Assert.AreEqual(2, vrijednosti.Length);
        }

        [TestMethod]
        [DataRow(NacinPreuzimanja.LICNO_PREUZIMANJE, 0, "LICNO_PREUZIMANJE")]
        [DataRow(NacinPreuzimanja.DOSTAVA, 1, "DOSTAVA")]
        public void NacinPreuzimanja_VrijednostiINazivi_OdgovarajuDefiniciji(
            NacinPreuzimanja opcija, int ocekivaniBroj, string ocekivaniNaziv)
        {
            // Assert
            Assert.AreEqual(ocekivaniBroj, (int)opcija);
            Assert.AreEqual(ocekivaniNaziv, opcija.ToString());
            Assert.IsTrue(Enum.IsDefined(typeof(NacinPreuzimanja), opcija));
        }

        [TestMethod]
        [DataRow("LICNO_PREUZIMANJE", true, NacinPreuzimanja.LICNO_PREUZIMANJE)]
        [DataRow("DOSTAVA", true, NacinPreuzimanja.DOSTAVA)]
        [DataRow("NEPOSTOJECA_OPCIJA", false, default(NacinPreuzimanja))]
        public void TryParse_RazlicitiUlazniStringovi_IspravnoParsira(
            string unos, bool ocekivaniUspjeh, NacinPreuzimanja ocekivanaVrijednost)
        {
            // Act
            bool uspjeh = Enum.TryParse<NacinPreuzimanja>(unos, out var rezultat);

            // Assert
            Assert.AreEqual(ocekivaniUspjeh, uspjeh);
            Assert.AreEqual(ocekivanaVrijednost, rezultat);
        }
    }
}