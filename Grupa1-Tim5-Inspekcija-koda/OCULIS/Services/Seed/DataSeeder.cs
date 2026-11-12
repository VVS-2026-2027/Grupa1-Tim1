using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;

namespace OCULIS.Services.Seed
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Korisnik>>();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            foreach (var uloga in Uloge.Sve)
            {
                if (!await roleManager.RoleExistsAsync(uloga))
                    await roleManager.CreateAsync(new IdentityRole(uloga));
            }

            await SeedKorisniciAsync(userManager);
            await SeedPoslovniceAsync(context);
            await SeedProizvodiAsync(context);
            await SeedAkcijeAsync(context);
        }

        private static async Task SeedKorisniciAsync(UserManager<Korisnik> userManager)
        {
            await KreirajKorisnikaAsync(userManager, "admin@oculis.ba", "Admin", "Adminović", Uloge.Administrator);
            await KreirajKorisnikaAsync(userManager, "zaposlenik@oculis.ba", "Emina", "Zaposlenić", Uloge.Zaposlenik);
            await KreirajKorisnikaAsync(userManager, "kupac@oculis.ba", "Amila", "Kupčević", Uloge.Kupac);
        }

        private static async Task KreirajKorisnikaAsync(
            UserManager<Korisnik> userManager, string email, string ime, string prezime, string uloga)
        {
            if (await userManager.FindByEmailAsync(email) != null) return;

            var korisnik = new Korisnik
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                Ime = ime,
                Prezime = prezime,
                Telefon = "+387 33 000 000"
            };

            var result = await userManager.CreateAsync(korisnik, "Oculis123!");
            if (result.Succeeded)
                await userManager.AddToRoleAsync(korisnik, uloga);
        }

        private static async Task SeedPoslovniceAsync(ApplicationDbContext context)
        {
            if (await context.Poslovnica.AnyAsync()) return;

            context.Poslovnica.AddRange(
                new Poslovnica
                {
                    Naziv = "OCULIS Centar Sarajevo",
                    Adresa = "Ferhadija 12, Sarajevo",
                    Telefon = "+387 33 123 456",
                    RadnoVrijeme = "Pon-Pet 08:00-20:00, Sub 09:00-15:00",
                    Latitude = 43.8563,
                    Longitude = 18.4131
                },
                new Poslovnica
                {
                    Naziv = "OCULIS Ilidža",
                    Adresa = "Butmirska cesta 14, Ilidža",
                    Telefon = "+387 33 234 567",
                    RadnoVrijeme = "Pon-Sub 09:00-19:00",
                    Latitude = 43.8286,
                    Longitude = 18.3103
                },
                new Poslovnica
                {
                    Naziv = "OCULIS Zenica",
                    Adresa = "Kralja Tvrtka 5, Zenica",
                    Telefon = "+387 32 345 678",
                    RadnoVrijeme = "Pon-Pet 08:00-18:00",
                    Latitude = 44.2014,
                    Longitude = 17.9064
                });
            await context.SaveChangesAsync();
        }

        private static async Task SeedProizvodiAsync(ApplicationDbContext context)
        {
            if (await context.Proizvod.AnyAsync()) return;

            context.Proizvod.AddRange(
                new Proizvod { Naziv = "Ray-Ban Aviator", Opis = "Klasične sunčane naočale", Cijena = 289.99, Kategorija = KategorijaProizvoda.SuncaneNaocale, Proizvodjac = "Ray-Ban", DostupnaKolicina = 15 },
                new Proizvod { Naziv = "Oakley Holbrook", Opis = "Sportski model sunčanih naočala", Cijena = 199.99, Kategorija = KategorijaProizvoda.SuncaneNaocale, Proizvodjac = "Oakley", DostupnaKolicina = 20 },
                new Proizvod { Naziv = "Essilor Varilux", Opis = "Dioptrijska stakla premium klase", Cijena = 450.00, Kategorija = KategorijaProizvoda.DioptrijskeNaocale, Proizvodjac = "Essilor", DostupnaKolicina = 10 },
                new Proizvod { Naziv = "Acuvue Oasys", Opis = "Kontaktna sočiva, pakovanje 6 kom", Cijena = 59.99, Kategorija = KategorijaProizvoda.KontaktnaSociva, Proizvodjac = "Johnson & Johnson", DostupnaKolicina = 50 },
                new Proizvod { Naziv = "Set za čišćenje sočiva", Opis = "Tečnost i kutija za sočiva", Cijena = 14.99, Kategorija = KategorijaProizvoda.DodatnaOprema, Proizvodjac = "OCULIS", DostupnaKolicina = 100 },
                new Proizvod { Naziv = "Vogue VO4195S", Opis = "Moderne dioptrijske naočale", Cijena = 179.99, Kategorija = KategorijaProizvoda.DioptrijskeNaocale, Proizvodjac = "Vogue", DostupnaKolicina = 12 });
            await context.SaveChangesAsync();
        }

        private static async Task SeedAkcijeAsync(ApplicationDbContext context)
        {
            if (await context.Akcija.AnyAsync()) return;

            var proizvod = await context.Proizvod.FirstOrDefaultAsync();
            context.Akcija.AddRange(
                new Akcija
                {
                    Naziv = "Proljetna akcija",
                    PopustPostotak = 20,
                    DatumPocetka = DateTime.Today.AddDays(-7),
                    DatumZavrsetka = DateTime.Today.AddDays(30),
                    Aktivna = true
                },
                new Akcija
                {
                    Naziv = "Popust na Ray-Ban",
                    PopustPostotak = 15,
                    DatumPocetka = DateTime.Today.AddDays(-3),
                    DatumZavrsetka = DateTime.Today.AddDays(14),
                    Aktivna = true,
                    IdProizvod = proizvod?.Id
                });
            await context.SaveChangesAsync();
        }
    }
}
