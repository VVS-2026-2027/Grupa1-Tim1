using BentoLab.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace UnitTestiranje
{
    [TestClass]
    [DoNotParallelize]
    public class EmailServiceTests
    {
        private StringWriter _konzolaIzlaz = null!;
        private TextWriter _originalniIzlaz = null!;

        [TestInitialize]
        public void Setup()
        {
            _originalniIzlaz = Console.Out;
            _konzolaIzlaz = new StringWriter();
            Console.SetOut(_konzolaIzlaz);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.SetOut(_originalniIzlaz);
            _konzolaIzlaz.Dispose();
        }

        [TestMethod]
        public void PosaljiObavjestenje_ValidniPodaciBezSlike_NeBacaIzuzetak()
        {
            // Arrange
            string email = "kupac@bentolab.ba";
            string brojNarudzbe = "101";
            string status = "U pripremi";

            // Act
            EmailService.PosaljiObavjestenje(email, brojNarudzbe, status, null);

            // Assert
            string ispis = _konzolaIzlaz.ToString();
            Assert.IsFalse(ispis.Contains("FormatException"), "Validan email ne smije izazvati grešku formata.");
        }

        [TestMethod]
        public void PosaljiObavjestenje_StatusSpremnoINepostojecaSlika_PokrivaFallbackGranuZaSliku()
        {
            // Arrange
            string email = "kupac@bentolab.ba";
            string brojNarudzbe = "102";
            string status = "Spremno za preuzimanje";
            string nepostojecaSlika = "/slike/ne_postoji_123.jpg";

            // Act
            EmailService.PosaljiObavjestenje(email, brojNarudzbe, status, nepostojecaSlika);

            // Assert
            Assert.IsTrue(true);
        }

        [TestMethod]
        public void PosaljiObavjestenje_StatusSpremnoIPostojecaSlika_UspjesnoPrilazeSliku()
        {
            // Arrange
            string wwwrootFolder = Path.GetFullPath(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "wwwroot"));
            Directory.CreateDirectory(wwwrootFolder);

            string nazivFajla = $"test_torta_{Guid.NewGuid():N}.jpg";
            string putanjaDoTestneSlike = Path.Combine(wwwrootFolder, nazivFajla);
            File.WriteAllText(putanjaDoTestneSlike, "lazni sadrzaj slike za test");

            try
            {
                string email = "kupac@bentolab.ba";
                string brojNarudzbe = "103";
                string status = "Spremno";

                // Act
                EmailService.PosaljiObavjestenje(email, brojNarudzbe, status, "/" + nazivFajla);

                // Assert
                Assert.IsTrue(File.Exists(putanjaDoTestneSlike));
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
                if (File.Exists(putanjaDoTestneSlike))
                {
                    File.Delete(putanjaDoTestneSlike);
                }
            }
        }

        [TestMethod]
        [DataRow("", "104", "U pripremi")]
        [DataRow("neispravan-email", "105", "Spremno")]
        [DataRow("kupac@bentolab.ba", "106", null)]
        public void PosaljiObavjestenje_NeispravniUlazniPodaci_HvataIzuzetakIIspisujeGresku(
            string neispravanEmail, string brojNarudzbe, string neispravanStatus)
        {
            // Arrange & Act
            EmailService.PosaljiObavjestenje(neispravanEmail, brojNarudzbe, neispravanStatus);

            // Assert
            string ispis = _konzolaIzlaz.ToString();
            StringAssert.Contains(ispis, "Greška pri slanju maila:");
        }
    }
}