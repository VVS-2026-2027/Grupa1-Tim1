using BentoLab.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace UnitTestiranje
{
	[TestClass]
	public class DekoracijaTests
	{
		[TestMethod]
		public void Dekoracija_NoviObjekat_ImaPodrazumijevaneVrijednosti()
		{
			// Arrange & Act
			var dekoracija = new Dekoracija();

			// Assert
			Assert.AreEqual(0, dekoracija.DekoracijaID);
			Assert.IsNull(dekoracija.Naziv);
			Assert.AreEqual(0.0, dekoracija.Cijena);
			Assert.AreEqual(default(TipDekoracije), dekoracija.TipDekoracije);
		}

		[TestMethod]
		public void Dekoracija_PostavljanjeIDa_VracaIspravanID()
		{
			// Arrange
			var dekoracija = new Dekoracija();

			// Act
			dekoracija.DekoracijaID = 5;

			// Assert
			Assert.AreEqual(5, dekoracija.DekoracijaID);
		}

		[TestMethod]
		public void Dekoracija_PostavljanjeNaziva_VracaIspravanNaziv()
		{
			// Arrange
			var dekoracija = new Dekoracija();

			// Act
			dekoracija.Naziv = "Cvjetna dekoracija";

			// Assert
			Assert.AreEqual("Cvjetna dekoracija", dekoracija.Naziv);
		}

		[TestMethod]
		[DataRow(0.0)]
		[DataRow(10.50)]
		[DataRow(100.0)]
		[DataRow(-5.0)]
		public void Dekoracija_PostavljanjeCijene_VracaUnesenuVrijednost(double cijena)
		{
			// Arrange
			var dekoracija = new Dekoracija();

			// Act
			dekoracija.Cijena = cijena;

			// Assert
			Assert.AreEqual(cijena, dekoracija.Cijena);
		}

		
[DataTestMethod]
[DataRow(TipDekoracije.JESTIVE_SLJOKICE)]
[DataRow(TipDekoracije.JESTIVE_PERLICE)]
[DataRow(TipDekoracije.MASNICE)]
[DataRow(TipDekoracije.JESTIVO_CVIJECE)]
[DataRow(TipDekoracije.COKOLADNA_FIGURICA)]
public void Dekoracija_PostavljanjeTipa_VracaIspravanTip(
    TipDekoracije tip)
{
    // Arrange
    var dekoracija = new Dekoracija();

    // Act
    dekoracija.TipDekoracije = tip;

    // Assert
    Assert.AreEqual(tip, dekoracija.TipDekoracije);
}


		[TestMethod]
		public void Dekoracija_DekoracijaID_ImaKeyAtribut()
		{
			// Arrange
			var svojstvo = typeof(Dekoracija)
				.GetProperty(nameof(Dekoracija.DekoracijaID));

			// Act
			var atributi = svojstvo!
				.GetCustomAttributes(typeof(KeyAttribute), false);

			// Assert
			Assert.IsTrue(atributi.Any());
		}
	}
}
