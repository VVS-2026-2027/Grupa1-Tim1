using BentoLab.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestiranje
{
	[TestClass]
	public class ErrorViewModelTests
	{
		[TestMethod]
		public void ErrorViewModel_NoviObjekat_NemaRequestId()
		{
			// Arrange & Act
			var model = new ErrorViewModel();

			// Assert
			Assert.IsNull(model.RequestId);
			Assert.IsFalse(model.ShowRequestId);
		}

		[TestMethod]
		[DataRow("ABC123")]
		[DataRow("12345")]
		[DataRow(" ")]
		[DataRow("request-2026-001")]
		public void ShowRequestId_NeprazanRequestId_VracaTrue(string requestId)
		{
			// Arrange
			var model = new ErrorViewModel();

			// Act
			model.RequestId = requestId;

			// Assert
			Assert.IsTrue(model.ShowRequestId);
		}

		[TestMethod]
		[DataRow("")]
		[DataRow(null)]
		public void ShowRequestId_PrazanIliNullRequestId_VracaFalse(string? requestId)
		{
			// Arrange
			var model = new ErrorViewModel();

			// Act
			model.RequestId = requestId;

			// Assert
			Assert.IsFalse(model.ShowRequestId);
		}

		[TestMethod]
		public void ShowRequestId_PromjenaRequestId_AzuriraRezultat()
		{
			// Arrange
			var model = new ErrorViewModel
			{
				RequestId = "ABC123"
			};

			// Assert
			Assert.IsTrue(model.ShowRequestId);

			// Act
			model.RequestId = "";

			// Assert
			Assert.IsFalse(model.ShowRequestId);
		}
	}
}
