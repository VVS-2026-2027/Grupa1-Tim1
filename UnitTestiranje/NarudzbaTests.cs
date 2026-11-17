using BentoLab.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace UnitTestiranje
{
    [TestClass]
    public class NarudzbaTests
    {
        [TestMethod]
        public void Konstruktor_PodrazumijevaneVrijednosti_PostavljaPocetnoStanje()
        {
            // Act
            var narudzba = new Narudzba();

            // Assert
            Assert.AreEqual(0, narudzba.NarudzbaID);
            Assert.AreEqual(0.0, narudzba.UkupnaCijena);
            Assert.AreEqual(0.0, narudzba.KoeficijentSlozenosti);
            Assert.AreEqual(default(StatusNarudzbe), narudzba.Status);
            Assert.AreEqual(default(DateTime), narudzba.DatumNarudzbe);
            Assert.AreEqual(default(DateTime), narudzba.DatumPreuzimanja);
            Assert.AreEqual(NacinPreuzimanja.LICNO_PREUZIMANJE, narudzba.NacinPreuzimanja);
            Assert.AreEqual(0, narudzba.KorisnikID);
            Assert.IsNull(narudzba.Korisnik);
            Assert.IsNull(narudzba.Stavke);
            Assert.IsNull(narudzba.Dostava);
            Assert.IsNull(narudzba.Obavjestenja);
        }

        [TestMethod]
        [DataRow(1, 45.50, 1.2, 0, NacinPreuzimanja.LICNO_PREUZIMANJE, 10, "2026-11-13T10:00:00", "2026-11-15T14:00:00")]
        [DataRow(15, 120.00, 1.8, 1, NacinPreuzimanja.DOSTAVA, 22, "2026-11-14T09:30:00", "2026-11-16T16:30:00")]
        public void SvojstvaNarudzbe_PostavljanjeICitanjeVrijednosti_VracaIspravnePodatke(
            int narudzbaId,
            double ukupnaCijena,
            double koeficijent,
            int statusVrijednost,
            NacinPreuzimanja nacinPreuzimanja,
            int korisnikId,
            string datumNarudzbeTekst,
            string datumPreuzimanjaTekst)
        {
            // Arrange
            var narudzba = new Narudzba();
            var status = (StatusNarudzbe)statusVrijednost;
            var datumNarudzbe = DateTime.Parse(datumNarudzbeTekst);
            var datumPreuzimanja = DateTime.Parse(datumPreuzimanjaTekst);
            var kupac = new Korisnik { Id = korisnikId, KorisnikID = korisnikId };
            var stavke = new List<StavkaNarudzbe>();
            var dostava = new Dostava();
            var obavjestenja = new List<Obavjestenje>();

            // Act
            narudzba.NarudzbaID = narudzbaId;
            narudzba.UkupnaCijena = ukupnaCijena;
            narudzba.KoeficijentSlozenosti = koeficijent;
            narudzba.Status = status;
            narudzba.DatumNarudzbe = datumNarudzbe;
            narudzba.DatumPreuzimanja = datumPreuzimanja;
            narudzba.NacinPreuzimanja = nacinPreuzimanja;
            narudzba.KorisnikID = korisnikId;
            narudzba.Korisnik = kupac;
            narudzba.Stavke = stavke;
            narudzba.Dostava = dostava;
            narudzba.Obavjestenja = obavjestenja;

            // Assert
            Assert.AreEqual(narudzbaId, narudzba.NarudzbaID);
            Assert.AreEqual(ukupnaCijena, narudzba.UkupnaCijena);
            Assert.AreEqual(koeficijent, narudzba.KoeficijentSlozenosti);
            Assert.AreEqual(status, narudzba.Status);
            Assert.AreEqual(datumNarudzbe, narudzba.DatumNarudzbe);
            Assert.AreEqual(datumPreuzimanja, narudzba.DatumPreuzimanja);
            Assert.AreEqual(nacinPreuzimanja, narudzba.NacinPreuzimanja);
            Assert.AreEqual(korisnikId, narudzba.KorisnikID);
            Assert.AreSame(kupac, narudzba.Korisnik);
            Assert.AreSame(stavke, narudzba.Stavke);
            Assert.AreSame(dostava, narudzba.Dostava);
            Assert.AreSame(obavjestenja, narudzba.Obavjestenja);
        }

        [TestMethod]
        public void AtributiModela_ProvjeraPrimarnogIStranogKljuca_IspravnoKonfigurisano()
        {
            // Arrange
            PropertyInfo? narudzbaIdSvojstvo = typeof(Narudzba).GetProperty(nameof(Narudzba.NarudzbaID));
            PropertyInfo? korisnikSvojstvo = typeof(Narudzba).GetProperty(nameof(Narudzba.Korisnik));

            Assert.IsNotNull(narudzbaIdSvojstvo);
            Assert.IsNotNull(korisnikSvojstvo);

            // Act
            var keyAtribut = narudzbaIdSvojstvo.GetCustomAttribute<KeyAttribute>();
            var foreignKeyAtribut = korisnikSvojstvo.GetCustomAttribute<ForeignKeyAttribute>();

            // Assert
            Assert.IsNotNull(keyAtribut, "Svojstvo NarudzbaID mora imati [Key] atribut.");
            Assert.IsNotNull(foreignKeyAtribut, "Svojstvo Korisnik mora imati [ForeignKey] atribut.");
            Assert.AreEqual("KorisnikID", foreignKeyAtribut.Name);
        }
    }
}