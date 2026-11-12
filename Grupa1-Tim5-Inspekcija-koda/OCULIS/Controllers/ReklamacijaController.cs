using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;

namespace OCULIS.Controllers
{
    [Authorize]
    public class ReklamacijaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IEmailSender _emailSender;

        public ReklamacijaController(
            ApplicationDbContext context,
            UserManager<Korisnik> userManager,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailSender = emailSender;
        }

        public async Task<IActionResult> Index()
        {
            var query = _context.Reklamacija
                .Include(r => r.Korisnik)
                .Include(r => r.Narudzba)
                .AsQueryable();

            if (User.IsInRole(Uloge.Kupac))
            {
                var user = await _userManager.GetUserAsync(User);
                query = query.Where(r => r.IdKorisnik == user!.Id);
            }

            return View(await query.OrderByDescending(r => r.DatumPodnosenja).ToListAsync());
        }

        [Authorize(Roles = Uloge.Kupac)]
        public async Task<IActionResult> Create()
        {
            await PopuniNarudzbeAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Uloge.Kupac)]
        public async Task<IActionResult> Create([Bind("Opis,IdNarudzba")] Reklamacija reklamacija)
        {
            var user = await _userManager.GetUserAsync(User);

            reklamacija.IdKorisnik = user!.Id;
            reklamacija.DatumPodnosenja = DateTime.Now;
            reklamacija.Status = StatusReklamacije.Podnesena;

            var karton = await _context.ElektronskiKarton
                .FirstOrDefaultAsync(e => e.IdKorisnik == user.Id);

            reklamacija.IdElektronskiKarton = karton?.Id;

            ModelState.Remove(nameof(Reklamacija.IdKorisnik));
            ModelState.Remove(nameof(Reklamacija.Korisnik));
            ModelState.Remove(nameof(Reklamacija.DatumPodnosenja));
            ModelState.Remove(nameof(Reklamacija.Status));
            ModelState.Remove(nameof(Reklamacija.IdElektronskiKarton));
            ModelState.Remove(nameof(Reklamacija.ElektronskiKarton));
            ModelState.Remove(nameof(Reklamacija.Narudzba));
            ModelState.Remove(nameof(Reklamacija.Odgovor));

            if (string.IsNullOrWhiteSpace(reklamacija.Opis))
            {
                ModelState.AddModelError(nameof(Reklamacija.Opis), "Molimo unesite opis reklamacije.");
            }

            if (!ModelState.IsValid)
            {
                await PopuniNarudzbeAsync(reklamacija.IdNarudzba);
                return View(reklamacija);
            }

            _context.Reklamacija.Add(reklamacija);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var narudzbaTekst = reklamacija.IdNarudzba.HasValue
                    ? $"Narudžba #{reklamacija.IdNarudzba.Value}"
                    : "Reklamacija nije vezana za konkretnu narudžbu.";

                var emailBody = $@"
                    <div style='font-family: Arial, sans-serif; color: #222; line-height: 1.6;'>
                        <h2 style='color: #8f672e;'>OCULIS - Reklamacija zaprimljena</h2>

                        <p>Poštovani,</p>

                        <p>
                            Vaša reklamacija je uspješno zaprimljena.
                            Naš tim će pregledati zahtjev i kontaktirati vas u najkraćem mogućem roku.
                        </p>

                        <div style='background: #fbf7ef; border: 1px solid #e8d8bd; padding: 16px; border-radius: 10px; margin: 18px 0;'>
                            <p><strong>Status:</strong> Podnesena</p>
                            <p><strong>Datum podnošenja:</strong> {reklamacija.DatumPodnosenja:dd.MM.yyyy. HH:mm}</p>
                            <p><strong>Narudžba:</strong> {narudzbaTekst}</p>
                            <p><strong>Opis reklamacije:</strong></p>
                            <p>{reklamacija.Opis}</p>
                        </div>

                        <p>
                            Hvala vam na povjerenju.<br />
                            <strong>OCULIS Optika</strong>
                        </p>
                    </div>
                ";

                await _emailSender.SendEmailAsync(
                    user.Email,
                    "OCULIS - Reklamacija je zaprimljena",
                    emailBody
                );
            }

            TempData["Success"] = "Reklamacija je uspješno poslana.";
            TempData["Info"] = "Naš tim će pregledati vaš zahtjev i kontaktirati vas u najkraćem mogućem roku.";

            return RedirectToAction(nameof(Potvrda));
        }

        [Authorize(Roles = Uloge.Kupac)]
        public IActionResult Potvrda()
        {
            return View();
        }

        [Authorize(Roles = $"{Uloge.Zaposlenik},{Uloge.Administrator}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var reklamacija = await _context.Reklamacija.FindAsync(id);

            if (reklamacija == null) return NotFound();

            return View(reklamacija);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Uloge.Zaposlenik},{Uloge.Administrator}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status,Odgovor")] Reklamacija model)
        {
            var reklamacija = await _context.Reklamacija.FindAsync(id);

            if (reklamacija == null) return NotFound();

            reklamacija.Status = model.Status;
            reklamacija.Odgovor = model.Odgovor;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Reklamacija je uspješno ažurirana.";
            return RedirectToAction(nameof(Index));
        }

        private async Task PopuniNarudzbeAsync(int? odabranaNarudzba = null)
        {
            var user = await _userManager.GetUserAsync(User);

            var narudzbe = await _context.Narudzba
                .Where(n => n.IdKorisnik == user!.Id)
                .OrderByDescending(n => n.Id)
                .Select(n => new
                {
                    n.Id,
                    Naziv = "Narudžba #" + n.Id
                })
                .ToListAsync();

            ViewData["IdNarudzba"] = new SelectList(narudzbe, "Id", "Naziv", odabranaNarudzba);
        }
    }
}