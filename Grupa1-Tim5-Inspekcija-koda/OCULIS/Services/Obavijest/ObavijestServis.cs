using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using OCULIS.Data;

namespace OCULIS.Services.Obavijest
{
    public interface IObavijestServis
    {
        Task PosaljiObavijestAsync(string korisnikId, string naslov, string tekst, bool posaljiEmail = true);
        Task PosaljiObavijestNarudzbaAsync(string korisnikId, int narudzbaId);
        Task PosaljiObavijestTerminAsync(string korisnikId, int terminId);
    }

    public class ObavijestServis : IObavijestServis
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IServiceScopeFactory _scopeFactory;

        public ObavijestServis(
            ApplicationDbContext context,
            IEmailSender emailSender,
            IServiceScopeFactory scopeFactory)
        {
            _context = context;
            _emailSender = emailSender;
            _scopeFactory = scopeFactory;
        }

        public async Task PosaljiObavijestAsync(string korisnikId, string naslov, string tekst, bool posaljiEmail = true)
        {
            var obavijest = new Models.Obavijest
            {
                IdKorisnik = korisnikId,
                Naslov = naslov,
                Tekst = tekst,
                DatumSlanja = DateTime.Now,
                PoslanoEmailom = false
            };

            _context.Obavijest.Add(obavijest);
            await _context.SaveChangesAsync();

            if (posaljiEmail)
            {
                _ = Task.Run(async () =>
                {
                    using var scope = _scopeFactory.CreateScope();
                    var ctx = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    var email = scope.ServiceProvider.GetRequiredService<IEmailSender>();

                    var korisnik = await ctx.Users.FindAsync(korisnikId);
                    if (korisnik?.Email != null)
                    {
                        await email.SendEmailAsync(korisnik.Email, naslov, tekst);
                        var saved = await ctx.Obavijest.FindAsync(obavijest.Id);
                        if (saved != null)
                        {
                            saved.PoslanoEmailom = true;
                            await ctx.SaveChangesAsync();
                        }
                    }
                });
            }
        }

        public async Task PosaljiObavijestNarudzbaAsync(string korisnikId, int narudzbaId)
        {
            await PosaljiObavijestAsync(
                korisnikId,
                "Potvrda narudžbe - OCULIS",
                $"Vaša narudžba #{narudzbaId} je uspješno kreirana. Status možete pratiti u svom profilu.");
        }

        public async Task PosaljiObavijestTerminAsync(string korisnikId, int terminId)
        {
            var termin = await _context.TerminPregleda
                .Include(t => t.Poslovnica)
                .FirstOrDefaultAsync(t => t.Id == terminId);

            var lokacija = termin?.Poslovnica?.Naziv ?? "poslovnici";
            await PosaljiObavijestAsync(
                korisnikId,
                "Potvrda termina pregleda - OCULIS",
                $"Vaš termin pregleda #{terminId} je zakazan u {lokacija} dana {termin?.Datum:dd.MM.yyyy} u {termin?.Vrijeme:hh\\:mm}.");
        }
    }
}
