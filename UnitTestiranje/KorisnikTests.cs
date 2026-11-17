using BentoLab.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace UnitTestiranje
{
    [TestClass]
    public class KorisnikTests
    {
        [TestMethod]
        public void Konstruktor_PodrazumijevaneVrijednosti_PostavljaImePrezimeIUloguKupac()
        {
            // Act
            var korisnik = new Korisnik();

            // Assert
            Assert.AreEqual("Registrovani korisnik", korisnik.ImePrezime);
            Assert.AreEqual(Uloga.KUPAC, korisnik.Uloga);
            Assert.AreEqual(0, korisnik.KorisnikID);
            Assert.IsNull(korisnik.Lozinka);
            Assert.IsNull(korisnik.Narudzbe);
            Assert.IsNull(korisnik.Korpa);
            Assert.IsNull(korisnik.Obavjestenja);
        }

        [TestMethod]
        [DataRow(1, "Amra Kovač", "amra@bentolab.ba", "TajnaLozinka123!", Uloga.KUPAC)]
        [DataRow(2, "Admin BentoLab", "admin@bentolab.ba", "AdminPass99!", Uloga.ADMIN)]
        public void SvojstvaKorisnika_PostavljanjeICitanjeVrijednosti_VracaIspravnePodatke(
            int id, string imePrezime, string email, string lozinka, Uloga uloga)
        {
            // Arrange
            var korisnik = new Korisnik();
            var listaNarudzbi = new List<Narudzba>();
            var listaObavjestenja = new List<Obavjestenje>();
            var korpa = new Korpa();

            // Act
            korisnik.Id = id;
            korisnik.KorisnikID = id;
            korisnik.ImePrezime = imePrezime;
            korisnik.Email = email;
            korisnik.UserName = email;
            korisnik.Lozinka = lozinka;
            korisnik.Uloga = uloga;
            korisnik.Narudzbe = listaNarudzbi;
            korisnik.Obavjestenja = listaObavjestenja;
            korisnik.Korpa = korpa;

            // Assert
            Assert.AreEqual(id, korisnik.Id);
            Assert.AreEqual(id, korisnik.KorisnikID);
            Assert.AreEqual(imePrezime, korisnik.ImePrezime);
            Assert.AreEqual(email, korisnik.Email);
            Assert.AreEqual(email, korisnik.UserName);
            Assert.AreEqual(lozinka, korisnik.Lozinka);
            Assert.AreEqual(uloga, korisnik.Uloga);
            Assert.AreSame(listaNarudzbi, korisnik.Narudzbe);
            Assert.AreSame(listaObavjestenja, korisnik.Obavjestenja);
            Assert.AreSame(korpa, korisnik.Korpa);
        }

        [TestMethod]
        [DataRow(nameof(Korisnik.KorisnikID), true)]
        [DataRow(nameof(Korisnik.Lozinka), true)]
        [DataRow(nameof(Korisnik.ImePrezime), false)]
        [DataRow(nameof(Korisnik.Uloga), false)]
        public void NotMappedAtribut_ProvjeraSvojstava_IspravnoKonfigurisanoZaBazu(
            string nazivSvojstva, bool ocekivanoNotMapped)
        {
            // Arrange
            PropertyInfo? svojstvo = typeof(Korisnik).GetProperty(nazivSvojstva);
            Assert.IsNotNull(svojstvo);

            // Act
            var notMappedAtribut = svojstvo.GetCustomAttribute<NotMappedAttribute>();

            // Assert
            if (ocekivanoNotMapped)
            {
                Assert.IsNotNull(notMappedAtribut, $"Svojstvo {nazivSvojstva} mora imati [NotMapped] atribut.");
            }
            else
            {
                Assert.IsNull(notMappedAtribut, $"Svojstvo {nazivSvojstva} ne smije imati [NotMapped] atribut.");
            }
        }
    }
}