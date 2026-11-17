using System;
using System.Threading.Tasks;
using BentoLab.Controllers;
using BentoLab.Data;
using BentoLab.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestiranje
{
    [TestClass]
    public class NarudzbaIzvrsavanjeTests
    {
        [TestMethod]
        public async Task OtkaziNarudzbu_KreiranaNarudzba_StatusPostajeOtkazanaUBazi()
        {
            // Arrange: upisi narudzbu u bazu (prvi context)
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                context.Narudzbe.Add(new Narudzba
                {
                    NarudzbaID = 100,
                    KorisnikID = 1,
                    Status = StatusNarudzbe.KREIRANA
                });
                await context.SaveChangesAsync();
            }

            // Act: kontroler dobija svoj context (drugi context)
            using (var context = new ApplicationDbContext(options))
            {
                var controller = new AdminNarudzbeController(context);
                await controller.OtkaziNarudzbu(100);
            }

            // Assert: treci, potpuno novi context cita iz baze
            using (var context = new ApplicationDbContext(options))
            {
                var narudzba = await context.Narudzbe.FindAsync(100);
                Assert.AreEqual(StatusNarudzbe.OTKAZANA, narudzba!.Status);
            }
        }
    }
}