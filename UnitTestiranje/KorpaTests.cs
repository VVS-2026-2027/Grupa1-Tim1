using BentoLab.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace UnitTestiranje
{
    [TestClass]
    public class KorpaTests
    {
        [TestMethod]
        public void Konstruktor_PodrazumijevaneVrijednosti_PostavljaPocetnoStanje()
        {
            // Act
            var korpa = new Korpa();

            // Assert
            Assert.AreEqual(0, korpa.KorpaID);
            Assert.AreEqual(0, korpa.KorisnikID);
            Assert.AreEqual(default(DateTime), korpa.DatumKreiranja);
            Assert.IsNull(korpa.Korisnik);
        }

        [TestMethod]
        [DataRow(1, 10, "2026-11-13T18:30:00")]
        [DataRow(25, 42, "2026-12-01T12:00:00")]
        public void SvojstvaKorpe_PostavljanjeICitanjeVrijednosti_VracaIspravnePodatke(
            int korpaId, int korisnikId, string datumTekst)
        {
            // Arrange
            var korpa = new Korpa();
            var datumKreiranja = DateTime.Parse(datumTekst);
            var kupac = new Korisnik
            {
                Id = korisnikId,
                KorisnikID = korisnikId,
                ImePrezime = "Testni Kupac"
            };

            // Act
            korpa.KorpaID = korpaId;
            korpa.KorisnikID = korisnikId;
            korpa.DatumKreiranja = datumKreiranja;
            korpa.Korisnik = kupac;

            // Assert
            Assert.AreEqual(korpaId, korpa.KorpaID);
            Assert.AreEqual(korisnikId, korpa.KorisnikID);
            Assert.AreEqual(datumKreiranja, korpa.DatumKreiranja);
            Assert.AreSame(kupac, korpa.Korisnik);
        }

        [TestMethod]
        public void AtributiModela_ProvjeraPrimarnogIStranogKljuca_IspravnoKonfigurisano()
        {
            // Arrange
            PropertyInfo? korpaIdSvojstvo = typeof(Korpa).GetProperty(nameof(Korpa.KorpaID));
            PropertyInfo? korisnikSvojstvo = typeof(Korpa).GetProperty(nameof(Korpa.Korisnik));

            Assert.IsNotNull(korpaIdSvojstvo);
            Assert.IsNotNull(korisnikSvojstvo);

            // Act
            var keyAtribut = korpaIdSvojstvo.GetCustomAttribute<KeyAttribute>();
            var foreignKeyAtribut = korisnikSvojstvo.GetCustomAttribute<ForeignKeyAttribute>();

            // Assert
            Assert.IsNotNull(keyAtribut, "Svojstvo KorpaID mora imati [Key] atribut.");
            Assert.IsNotNull(foreignKeyAtribut, "Svojstvo Korisnik mora imati [ForeignKey] atribut.");
            Assert.AreEqual("KorisnikID", foreignKeyAtribut.Name);
        }
    }
}