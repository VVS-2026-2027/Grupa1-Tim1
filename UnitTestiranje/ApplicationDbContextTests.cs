using BentoLab.Data;
using BentoLab.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTestiranje
{
    [TestClass]
    public class ApplicationDbContextTests
    {
        private ApplicationDbContext KreirajContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [TestMethod]
        public void DbSetovi_SuInicijalizovani()
        {
            using var context = KreirajContext();

            Assert.IsNotNull(context.Narudzbe);
            Assert.IsNotNull(context.StavkeNarudzbe);
            Assert.IsNotNull(context.Korpe);
            Assert.IsNotNull(context.Dostave);
            Assert.IsNotNull(context.Obavjestenja);
            Assert.IsNotNull(context.Korisnik);
            Assert.IsNotNull(context.Torta);
            Assert.IsNotNull(context.Dekoracija);
        }

        [TestMethod]
        public void OnModelCreating_ObavjestenjeStraniKljuceviImajuDeleteBehaviorNoAction()
        {
            using var context = KreirajContext();
            var entitet = context.Model.FindEntityType(typeof(Obavjestenje));

            Assert.IsNotNull(entitet);
            var straniKljucevi = entitet.GetForeignKeys().ToList();
            Assert.AreEqual(2, straniKljucevi.Count);
            Assert.IsTrue(straniKljucevi.All(fk => fk.DeleteBehavior == DeleteBehavior.NoAction));
        }

        [TestMethod]
        public async Task Baza_SnimanjeICitanjePovezanihEntiteta_RadiIspravno()
        {
            using var context = KreirajContext();
            var korisnik = new Korisnik
            {
                Id = 1,
                UserName = "testuser",
                Email = "test@bentolab.ba",
                ImePrezime = "Test Korisnik",
                Uloga = Uloga.KUPAC
            };
            var narudzba = new Narudzba
            {
                NarudzbaID = 1,
                KorisnikID = 1,
                UkupnaCijena = 60,
                Status = StatusNarudzbe.KREIRANA,
                DatumNarudzbe = DateTime.UtcNow,
                DatumPreuzimanja = DateTime.UtcNow.AddDays(2),
                NacinPreuzimanja = NacinPreuzimanja.LICNO_PREUZIMANJE
            };
            var obavjestenje = new Obavjestenje
            {
                ObavjestenjeID = 1,
                KorisnikID = 1,
                NarudzbaID = 1,
                Sadrzaj = "Narudzba je kreirana",
                DatumSlanja = DateTime.UtcNow
            };

            context.Users.Add(korisnik);
            context.Narudzbe.Add(narudzba);
            context.Obavjestenja.Add(obavjestenje);
            await context.SaveChangesAsync();

            var ucitano = await context.Obavjestenja
                .Include(o => o.Korisnik)
                .Include(o => o.Narudzba)
                .FirstOrDefaultAsync(o => o.ObavjestenjeID == 1);

            Assert.IsNotNull(ucitano);
            Assert.AreEqual("Test Korisnik", ucitano.Korisnik.ImePrezime);
            Assert.AreEqual(60, ucitano.Narudzba.UkupnaCijena);
        }
    }
}