using System;
using System.Threading;
using System.Threading.Tasks;
using BentoLab.Controllers;
using BentoLab.Data;
using BentoLab.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace UnitTestiranje
{
	[TestClass]
	public class AdminNarudzbeMockTests
	{
		[TestMethod]
		public async Task PromijeniStatus_GreskaPriCuvanju_BacaIzuzetak()
		{
			// Arrange
			var options = new DbContextOptionsBuilder<ApplicationDbContext>()
				.UseInMemoryDatabase(Guid.NewGuid().ToString())
				.Options;

			var mockContext = new Mock<ApplicationDbContext>(options)
			{
				CallBase = true
			};

			var narudzba = new Narudzba
			{
				NarudzbaID = 1,
				KorisnikID = 1,
				Status = StatusNarudzbe.KREIRANA
			};

			mockContext.Object.Narudzbe.Add(narudzba);
            await mockContext.Object.SaveChangesAsync();
            mockContext.Invocations.Clear();

			// Simuliramo gresku prilikom cuvanja
			mockContext
				.Setup(c => c.SaveChangesAsync(
					It.IsAny<CancellationToken>()))
				.ThrowsAsync(new DbUpdateException(
					"Simulirana greska baze podataka"));

			var controller =
				new AdminNarudzbeController(mockContext.Object);

			// Act + Assert
			await Assert.ThrowsExactlyAsync<DbUpdateException>(
				async () =>
					await controller.PromijeniStatus(
						1,
						"POTVRDJENA"
					)
			);

			// Provjeravamo da je kontroler pokusao sacuvati promjenu
			mockContext.Verify(
				c => c.SaveChangesAsync(
					It.IsAny<CancellationToken>()),
				Times.Once
			);
		}
	}
}
