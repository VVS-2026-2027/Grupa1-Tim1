using BentoLab.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace UnitTestiranje
{
	[TestClass]
	public class DostavaTests
	{
		[TestMethod]
		public void Dostava_NoviObjekat_ImaPodrazumijevaneVrijednosti()
		{
			// Arrange & Act
			var dostava = new Dostava();

			// Assert
			Assert.AreEqual(0, dostava.DostavaID);
			Assert.IsNull(dostava.Adresa);
			Assert.IsNull(dostava.KontaktTelefon);
			Assert.AreEqual(0.0, dostava.CijenaDostave);
			Assert.AreEqual(default(DateTime), dostava.VrijemeIsporuke);
			Assert.IsNull(dostava.Napomena);
			Assert.AreEqual(0, dostava.NarudzbaID);
			Assert.IsNull(dostava.Narudzba);
		}

		[TestMethod]
		public void Dostava_PostavljanjeIDa_VracaIspravanID()
		{
			// Arrange
			var dostava = new Dostava();

			// Act
			dostava.DostavaID = 10;

			// Assert
			Assert.AreEqual(10, dostava.DostavaID);
		}

		[TestMethod]
		[DataRow("Sarajevo, Zmaja od Bosne 5")]
		[DataRow("Zenica, Bulevar 12")]
		[DataRow("")]
		public void Dostava_PostavljanjeAdrese_VracaUnesenuAdresu(string adresa)
		{
			// Arrange
			var dostava = new Dostava();

			// Act
			dostava.Adresa = adresa;

			// Assert
			Assert.AreEqual(adresa, dostava.Adresa);
		}

		[TestMethod]
		[DataRow("061123456")]
		[DataRow("+38761123456")]
		[DataRow("")]
		public void Dostava_PostavljanjeTelefona_VracaUneseniTelefon(string telefon)
		{
			// Arrange
			var dostava = new Dostava();

			// Act
			dostava.KontaktTelefon = telefon;

			// Assert
			Assert.AreEqual(telefon, dostava.KontaktTelefon);
		}

		[TestMethod]
		[DataRow(0.0)]
		[DataRow(5.0)]
		[DataRow(10.50)]
		[DataRow(-5.0)]
		public void Dostava_PostavljanjeCijene_VracaUnesenuVrijednost(double cijena)
		{
			// Arrange
			var dostava = new Dostava();

			// Act
			dostava.CijenaDostave = cijena;

			// Assert
			Assert.AreEqual(cijena, dostava.CijenaDostave);
		}

		[TestMethod]
		public void Dostava_PostavljanjeVremenaIsporuke_VracaIspravnoVrijeme()
		{
			// Arrange
			var dostava = new Dostava();
			var vrijeme = new DateTime(2026, 11, 20, 14, 30, 0);

			// Act
			dostava.VrijemeIsporuke = vrijeme;

			// Assert
			Assert.AreEqual(vrijeme, dostava.VrijemeIsporuke);
		}

		[TestMethod]
		public void Dostava_PostavljanjeNapomene_VracaIspravnuNapomenu()
		{
			// Arrange
			var dostava = new Dostava();

			// Act
			dostava.Napomena = "Pozvati prije dolaska.";

			// Assert
			Assert.AreEqual("Pozvati prije dolaska.", dostava.Napomena);
		}

		[TestMethod]
		public void Dostava_PostavljanjeNarudzbaID_VracaIspravanID()
		{
			// Arrange
			var dostava = new Dostava();

			// Act
			dostava.NarudzbaID = 25;

			// Assert
			Assert.AreEqual(25, dostava.NarudzbaID);
		}

		[TestMethod]
		public void Dostava_PostavljanjeNarudzbe_CuvaReferencuNaNarudzbu()
		{
			// Arrange
			var dostava = new Dostava();
			var narudzba = new Narudzba();

			// Act
			dostava.Narudzba = narudzba;

			// Assert
			Assert.AreSame(narudzba, dostava.Narudzba);
		}

		[TestMethod]
		public void Dostava_DostavaID_ImaKeyAtribut()
		{
			// Arrange
			var svojstvo = typeof(Dostava)
				.GetProperty(nameof(Dostava.DostavaID));

			// Act
			var atributi = svojstvo!
				.GetCustomAttributes(typeof(KeyAttribute), false);

			// Assert
			Assert.IsTrue(atributi.Any());
		}

		[TestMethod]
		public void Dostava_Narudzba_ImaForeignKeyAtribut()
		{
			// Arrange
			var svojstvo = typeof(Dostava)
				.GetProperty(nameof(Dostava.Narudzba));

			// Act
			var atribut = svojstvo!
				.GetCustomAttributes(typeof(ForeignKeyAttribute), false)
				.Cast<ForeignKeyAttribute>()
				.FirstOrDefault();

			// Assert
			Assert.IsNotNull(atribut);
			Assert.AreEqual("NarudzbaID", atribut.Name);
		}
	}
}
