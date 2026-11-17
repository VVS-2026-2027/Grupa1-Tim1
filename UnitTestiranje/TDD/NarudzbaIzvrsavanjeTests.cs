
using System;
using System.Threading.Tasks;
using BentoLab.Controllers;
using BentoLab.Data;
using BentoLab.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestiranje
{
    [TestClass]
    public class NarudzbaIzvrsavanjeTests
    {
        [TestMethod]
        public async Task OtkaziNarudzbu_StatusSeMijenjaUOtkazana()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var narudzba = new Narudzba
            {
                NarudzbaID = 100,
                KorisnikID = 1,
                Status = StatusNarudzbe.KREIRANA
            };

            context.Narudzbe.Add(narudzba);
            await context.SaveChangesAsync();

            var controller = new AdminNarudzbeController(context);

            // Act
            await controller.OtkaziNarudzbu(narudzba.NarudzbaID);

            // Assert
            var sacuvanaNarudzba = await context.Narudzbe
                .FindAsync(narudzba.NarudzbaID);

            Assert.AreEqual(
                StatusNarudzbe.OTKAZANA,
                sacuvanaNarudzba!.Status
            );
        }
    }
}
