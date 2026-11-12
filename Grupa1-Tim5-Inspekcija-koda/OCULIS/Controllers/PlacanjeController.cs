using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;
using OCULIS.Models.ViewModels;
using OCULIS.Services.Placanje;

namespace OCULIS.Controllers
{
    [Authorize]
    public class PlacanjeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IPlacanjeServisFactory _placanjeFactory;

        public PlacanjeController(
            ApplicationDbContext context,
            UserManager<Korisnik> userManager,
            IPlacanjeServisFactory placanjeFactory)
        {
            _context = context;
            _userManager = userManager;
            _placanjeFactory = placanjeFactory;
        }

        [Authorize(Roles = $"{Uloge.Zaposlenik},{Uloge.Administrator}")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Placanje
                .Include(p => p.Narudzba)
                .ThenInclude(n => n.Korisnik)
                .OrderByDescending(p => p.DatumPlacanja)
                .ToListAsync());
        }

        [Authorize(Roles = Uloge.Kupac)]
        public async Task<IActionResult> Plati(int narudzbaId)
        {
            var user = await _userManager.GetUserAsync(User);
            var narudzba = await _context.Narudzba
                .FirstOrDefaultAsync(n => n.Id == narudzbaId && n.IdKorisnik == user!.Id);

            if (narudzba == null) return NotFound();

            if (await _context.Placanje.AnyAsync(p => p.IdNarudzba == narudzbaId && p.StatusPlacanja == StatusPlacanja.Uspjesno))
            {
                TempData["Error"] = "Narudžba je već plaćena.";
                return RedirectToAction("Details", "Narudzba", new { id = narudzbaId });
            }

            return View(new PlacanjeViewModel
            {
                IdNarudzba = narudzba.Id,
                Iznos = narudzba.UkupnaCijena,
                StatusNarudzbe = narudzba.Status.ToString()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Uloge.Kupac)]
        public async Task<IActionResult> Plati(PlacanjeViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var narudzba = await _context.Narudzba
                .FirstOrDefaultAsync(n => n.Id == model.IdNarudzba && n.IdKorisnik == user!.Id);

            if (narudzba == null) return NotFound();

            if (model.NacinPlacanja == NacinPlacanja.KreditnaKartica && string.IsNullOrWhiteSpace(model.BrojKartice))
                ModelState.AddModelError(nameof(model.BrojKartice), "Broj kartice je obavezan za kartično plaćanje.");

            if (!ModelState.IsValid)
            {
                model.Iznos = narudzba.UkupnaCijena;
                model.StatusNarudzbe = narudzba.Status.ToString();
                return View(model);
            }

            var placanje = new Placanje
            {
                Iznos = narudzba.UkupnaCijena,
                DatumPlacanja = DateTime.Now,
                IdNarudzba = narudzba.Id,
                NacinPlacanja = model.NacinPlacanja,
                StatusPlacanja = StatusPlacanja.NaCekanju
            };

            var strategija = _placanjeFactory.Kreiraj(model.NacinPlacanja);
            var rezultat = await strategija.ObradiPlacanjeAsync(placanje, model.BrojKartice?.Replace(" ", ""));

            placanje.StatusPlacanja = rezultat.Status;
            placanje.ReferencaTransakcije = rezultat.ReferencaTransakcije;

            _context.Placanje.Add(placanje);

            if (rezultat.Uspjesno)
            {
                narudzba.Status = StatusNarudzbe.UObradi;
                TempData["Success"] = rezultat.Poruka;
            }
            else
            {
                TempData["Error"] = rezultat.Poruka;
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Narudzba", new { id = narudzba.Id });
        }

        [Authorize(Roles = $"{Uloge.Zaposlenik},{Uloge.Administrator}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PotvrdiGotovinu(int id)
        {
            var placanje = await _context.Placanje
                .Include(p => p.Narudzba)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (placanje == null) return NotFound();

            placanje.StatusPlacanja = StatusPlacanja.Uspjesno;
            placanje.Narudzba.Status = StatusNarudzbe.UObradi;
            await _context.SaveChangesAsync();

            TempData["Success"] = "Gotovinsko plaćanje potvrđeno.";
            return RedirectToAction("Details", "Narudzba", new { id = placanje.IdNarudzba });
        }
    }
}
