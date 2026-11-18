using BentoLab.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestiranje
{
    [TestClass]
    public class StatusNarudzbeTests
    {
        [TestMethod]
        public void Enum_BrojVrijednosti_JePet()
        {
            var vrijednosti = Enum.GetValues<StatusNarudzbe>();

            Assert.AreEqual(5, vrijednosti.Length);
        }

        [TestMethod]
        public void Enum_PodrazumijevanaVrijednost_JeKreirana()
        {
            StatusNarudzbe status = default;

            Assert.AreEqual(StatusNarudzbe.KREIRANA, status);
        }

        [TestMethod]
        [DataRow(StatusNarudzbe.KREIRANA, 0)]
        [DataRow(StatusNarudzbe.POTVRDJENA, 1)]
        [DataRow(StatusNarudzbe.U_IZRADI, 2)]
        [DataRow(StatusNarudzbe.ZAVRSENA, 3)]
        [DataRow(StatusNarudzbe.OTKAZANA, 4)]
        public void Enum_Vrijednost_ImaOcekivaniBroj(StatusNarudzbe status, int ocekivani)
        {
            Assert.AreEqual(ocekivani, (int)status);
        }

        [TestMethod]
        [DataRow("KREIRANA", StatusNarudzbe.KREIRANA)]
        [DataRow("POTVRDJENA", StatusNarudzbe.POTVRDJENA)]
        [DataRow("U_IZRADI", StatusNarudzbe.U_IZRADI)]
        [DataRow("ZAVRSENA", StatusNarudzbe.ZAVRSENA)]
        [DataRow("OTKAZANA", StatusNarudzbe.OTKAZANA)]
        public void Parse_ValidanTekst_VracaOdgovarajuciStatus(string tekst, StatusNarudzbe ocekivani)
        {
            var rezultat = Enum.Parse<StatusNarudzbe>(tekst);

            Assert.AreEqual(ocekivani, rezultat);
        }

        [TestMethod]
        [DataRow("NEPOSTOJECI")]
        [DataRow("")]
        [DataRow("kreirana_")]
        public void TryParse_NevalidanTekst_VracaFalse(string tekst)
        {
            bool uspjeh = Enum.TryParse<StatusNarudzbe>(tekst, out _);

            Assert.IsFalse(uspjeh);
        }
    }
}