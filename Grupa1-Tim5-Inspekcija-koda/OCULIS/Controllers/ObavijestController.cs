using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;
using OCULIS.Services.Obavijest;

namespace OCULIS.Controllers
{
    [Authorize]
    public class ObavijestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IObavijestServis _obavijestServis;

        public ObavijestController(
            ApplicationDbContext context,
            UserManager<Korisnik> userManager,
            IObavijestServis obavijestServis)
        {
            _context = context;
            _userManager = userManager;
            _obavijestServis = obavijestServis;
        }

        public async Task<IActionResult> Index()
        {
            var query = _context.Obavijest.Include(o => o.Korisnik).AsQueryable();

            if (User.IsInRole(Uloge.Kupac))
            {
                var user = await _userManager.GetUserAsync(User);
                query = query.Where(o => o.IdKorisnik == user!.Id);
            }

            return View(await query.OrderByDescending(o => o.DatumSlanja).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var obavijest = await _context.Obavijest
                .Include(o => o.Korisnik)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (obavijest == null) return NotFound();

            if (User.IsInRole(Uloge.Kupac))
            {
                var user = await _userManager.GetUserAsync(User);
                if (obavijest.IdKorisnik != user!.Id) return Forbid();
            }

            return View(obavijest);
        }

        [Authorize(Roles = Uloge.Administrator)]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Uloge.Administrator)]
        public async Task<IActionResult> Create(string emailKorisnika, string naslov, string tekst)
        {
            var korisnik = await _userManager.FindByEmailAsync(emailKorisnika);
            if (korisnik == null)
            {
                ModelState.AddModelError(string.Empty, "Korisnik sa unesenim emailom nije pronađen.");
                return View();
            }

            await _obavijestServis.PosaljiObavijestAsync(korisnik.Id, naslov, tekst);
            TempData["Success"] = "Obavijest poslana.";
            return RedirectToAction(nameof(Index));
        }
    }
}
