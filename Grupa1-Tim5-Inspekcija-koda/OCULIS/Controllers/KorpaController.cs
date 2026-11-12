using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;
using OCULIS.Models.ViewModels;
using OCULIS.Services.Popust;

namespace OCULIS.Controllers
{
    [Authorize(Roles = $"{Uloge.Kupac},{Uloge.Administrator}")]
    public class KorpaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IPopustKalkulatorService _popustService;

        public KorpaController(
            ApplicationDbContext context,
            UserManager<Korisnik> userManager,
            IPopustKalkulatorService popustService)
        {
            _context = context;
            _userManager = userManager;
            _popustService = popustService;
        }

        public async Task<IActionResult> Index()
        {
            var korpa = await DohvatiIliKreirajKorpuAsync();
            var model = await MapirajKorpuAsync(korpa);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Dodaj(int proizvodId, int kolicina = 1)
        {
            var proizvod = await _context.Proizvod.FindAsync(proizvodId);
            if (proizvod == null) return NotFound();

            if (kolicina < 1 || kolicina > proizvod.DostupnaKolicina)
            {
                TempData["Error"] = "Neispravna količina.";
                return RedirectToAction(nameof(Index));
            }

            var korpa = await DohvatiIliKreirajKorpuAsync();
            var stavka = await _context.StavkaKorpe
                .FirstOrDefaultAsync(s => s.IdKorpa == korpa.Id && s.IdProizvod == proizvodId);

            if (stavka != null)
            {
                stavka.Kolicina += kolicina;
                stavka.Cijena = proizvod.Cijena;
            }
            else
            {
                _context.StavkaKorpe.Add(new StavkaKorpe
                {
                    IdKorpa = korpa.Id,
                    IdProizvod = proizvodId,
                    Kolicina = kolicina,
                    Cijena = proizvod.Cijena
                });
            }

            await AzurirajUkupnuCijenuKorpeAsync(korpa.Id);

            TempData["Success"] = "Proizvod je dodan u korpu.";
            TempData["ShowCartLink"] = true;
            var referer = Request.Headers["Referer"].ToString();

            if (!string.IsNullOrEmpty(referer))
            {
                return Redirect(referer);
            }

            return RedirectToAction("Index", "Proizvod");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Ukloni(int stavkaId)
        {
            var korpa = await DohvatiIliKreirajKorpuAsync();
            var stavka = await _context.StavkaKorpe
                .FirstOrDefaultAsync(s => s.Id == stavkaId && s.IdKorpa == korpa.Id);

            if (stavka != null)
            {
                _context.StavkaKorpe.Remove(stavka);
                await _context.SaveChangesAsync();
                await AzurirajUkupnuCijenuKorpeAsync(korpa.Id);
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Checkout()
        {
            var korpa = await DohvatiIliKreirajKorpuAsync();
            var stavke = await _context.StavkaKorpe.Where(s => s.IdKorpa == korpa.Id).ToListAsync();

            if (!stavke.Any())
            {
                TempData["Error"] = "Korpa je prazna.";
                return RedirectToAction(nameof(Index));
            }

            var user = await _userManager.GetUserAsync(User);
            var popust = await _popustService.IzracunajPopustAsync(user!.Id, korpa.Id);

            return View(new NarudzbaCheckoutViewModel
            {
                KorpaId = korpa.Id,
                Popust = popust,
                Korpa = await MapirajKorpuAsync(korpa)
            });
        }

        private async Task<Korpa> DohvatiIliKreirajKorpuAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var korpa = await _context.Korpa
                .Include(k => k.Stavke)
                .ThenInclude(s => s.Proizvod)
                .FirstOrDefaultAsync(k => k.IdKorisnik == user!.Id);

            if (korpa == null)
            {
                korpa = new Korpa { IdKorisnik = user!.Id, UkupnaCijena = 0 };
                _context.Korpa.Add(korpa);
                await _context.SaveChangesAsync();
            }

            return korpa;
        }

        private async Task<KorpaViewModel> MapirajKorpuAsync(Korpa korpa)
        {
            var stavke = await _context.StavkaKorpe
                .Include(s => s.Proizvod)
                .Where(s => s.IdKorpa == korpa.Id)
                .ToListAsync();

            var user = await _userManager.GetUserAsync(User);
            var popust = stavke.Any()
                ? await _popustService.IzracunajPopustAsync(user!.Id, korpa.Id)
                : null;

            return new KorpaViewModel
            {
                KorpaId = korpa.Id,
                UkupnaCijena = stavke.Sum(s => s.Cijena * s.Kolicina),
                Popust = popust,
                Stavke = stavke.Select(s => new StavkaKorpeViewModel
                {
                    Id = s.Id,
                    IdProizvod = s.IdProizvod,
                    NazivProizvoda = s.Proizvod.Naziv,
                    Kolicina = s.Kolicina,
                    Cijena = s.Cijena
                }).ToList()
            };
        }

        private async Task AzurirajUkupnuCijenuKorpeAsync(int korpaId)
        {
            var korpa = await _context.Korpa.FindAsync(korpaId);
            if (korpa == null) return;

            korpa.UkupnaCijena = await _context.StavkaKorpe
                .Where(s => s.IdKorpa == korpaId)
                .SumAsync(s => s.Cijena * s.Kolicina);

            await _context.SaveChangesAsync();
        }
    }
}
