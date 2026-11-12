using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;
using OCULIS.Models.ViewModels;
using OCULIS.Services.Obavijest;
using OCULIS.Services.Termin;

namespace OCULIS.Controllers
{
    [Authorize]
    public class TerminPregledaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;
        private readonly ITerminServis _terminServis;
        private readonly IObavijestServis _obavijestServis;

        public TerminPregledaController(
            ApplicationDbContext context,
            UserManager<Korisnik> userManager,
            ITerminServis terminServis,
            IObavijestServis obavijestServis)
        {
            _context = context;
            _userManager = userManager;
            _terminServis = terminServis;
            _obavijestServis = obavijestServis;
        }

        public async Task<IActionResult> Index()
        {
            var query = _context.TerminPregleda
                .Include(t => t.Korisnik)
                .Include(t => t.Poslovnica)
                .AsQueryable();

            if (User.IsInRole(Uloge.Kupac))
            {
                var user = await _userManager.GetUserAsync(User);
                query = query.Where(t => t.IdKorisnik == user!.Id);
            }

            return View(await query.OrderBy(t => t.Datum).ThenBy(t => t.Vrijeme).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var termin = await _context.TerminPregleda
                .Include(t => t.Korisnik)
                .Include(t => t.Poslovnica)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (termin == null) return NotFound();
            if (!await MozePristupitiAsync(termin)) return Forbid();

            return View(termin);
        }

        [Authorize(Roles = Uloge.Kupac)]
        public async Task<IActionResult> Zakazi()
        {
            var model = new TerminZakazivanjeViewModel();
            await PopuniDostupneTermineAsync(model);
            ViewData["IdPoslovnica"] = new SelectList(await _context.Poslovnica.ToListAsync(), "Id", "Naziv");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Uloge.Kupac)]
        public async Task<IActionResult> Zakazi(TerminZakazivanjeViewModel model)
        {
            if (!await _terminServis.JeTerminDostupanAsync(model.IdPoslovnica, model.Datum, model.Vrijeme))
            {
                ModelState.AddModelError(string.Empty, "Odabrani termin nije dostupan. Odaberite drugi termin.");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var termin = new TerminPregleda
                {
                    Datum = model.Datum.Date,
                    Vrijeme = model.Vrijeme,
                    Status = StatusTermina.Zakazan,
                    IdPoslovnica = model.IdPoslovnica,
                    IdKorisnik = user!.Id
                };

                _context.TerminPregleda.Add(termin);
                await _context.SaveChangesAsync();
                await _obavijestServis.PosaljiObavijestTerminAsync(user.Id, termin.Id);

                TempData["Success"] = "Termin uspješno zakazan.";
                return RedirectToAction(nameof(Details), new { id = termin.Id });
            }

            await PopuniDostupneTermineAsync(model);
            ViewData["IdPoslovnica"] = new SelectList(await _context.Poslovnica.ToListAsync(), "Id", "Naziv", model.IdPoslovnica);
            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = Uloge.Kupac)]
        public async Task<IActionResult> DostupniTermini(int poslovnicaId, DateTime datum)
        {
            var termini = await _terminServis.DohvatiDostupneTermineAsync(poslovnicaId, datum);
            return Json(termini.Select(t => t.ToString(@"hh\:mm")));
        }

        [Authorize(Roles = $"{Uloge.Zaposlenik},{Uloge.Administrator}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var termin = await _context.TerminPregleda.FindAsync(id);
            if (termin == null) return NotFound();
            return View(termin);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Uloge.Zaposlenik},{Uloge.Administrator}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] TerminPregleda model)
        {
            var termin = await _context.TerminPregleda.FindAsync(id);
            if (termin == null) return NotFound();

            termin.Status = model.Status;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Status termina ažuriran.";
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task PopuniDostupneTermineAsync(TerminZakazivanjeViewModel model)
        {
            if (model.IdPoslovnica > 0)
                model.DostupniTermini = await _terminServis.DohvatiDostupneTermineAsync(model.IdPoslovnica, model.Datum);
        }

        private async Task<bool> MozePristupitiAsync(TerminPregleda termin)
        {
            if (User.IsInRole(Uloge.Administrator) || User.IsInRole(Uloge.Zaposlenik))
                return true;

            var user = await _userManager.GetUserAsync(User);
            return user != null && termin.IdKorisnik == user.Id;
        }
    }
}
