using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;
using OCULIS.Models.ViewModels;

namespace OCULIS.Controllers
{
    [Authorize]
    public class ElektronskiKartonController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public ElektronskiKartonController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var query = _context.ElektronskiKarton.Include(e => e.Korisnik).AsQueryable();

            if (User.IsInRole(Uloge.Kupac))
            {
                var user = await _userManager.GetUserAsync(User);
                query = query.Where(e => e.IdKorisnik == user!.Id);
            }

            return View(await query.OrderByDescending(e => e.DatumKreiranja).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var karton = await _context.ElektronskiKarton
                .Include(e => e.Korisnik)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (karton == null) return NotFound();
            if (!await MozePristupitiAsync(karton)) return Forbid();

            var model = new ElektronskiKartonDetaljiViewModel
            {
                Karton = karton,
                Pregledi = await _context.PregledVida
                    .Where(p => p.IdElektronskiKarton == id)
                    .OrderByDescending(p => p.DatumPregleda)
                    .ToListAsync(),
                Narudzbe = await _context.Narudzba
                    .Where(n => n.IdKorisnik == karton.IdKorisnik)
                    .OrderByDescending(n => n.DatumNarudzbe)
                    .ToListAsync(),
                Reklamacije = await _context.Reklamacija
                    .Where(r => r.IdElektronskiKarton == id || r.IdKorisnik == karton.IdKorisnik)
                    .OrderByDescending(r => r.DatumPodnosenja)
                    .ToListAsync()
            };

            return View(model);
        }

        [Authorize(Roles = $"{Uloge.Zaposlenik},{Uloge.Administrator}")]
        public async Task<IActionResult> Create()
        {
            ViewData["IdKorisnik"] = new SelectList(
                await _userManager.Users.Select(u => new { u.Id, Ime = u.Ime + " " + u.Prezime }).ToListAsync(),
                "Id", "Ime");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Uloge.Zaposlenik},{Uloge.Administrator}")]
        public async Task<IActionResult> Create([Bind("Napomena,IdKorisnik")] ElektronskiKarton karton)
        {
            if (ModelState.IsValid)
            {
                karton.DatumKreiranja = DateTime.Now;
                _context.Add(karton);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Elektronski karton kreiran.";
                return RedirectToAction(nameof(Details), new { id = karton.Id });
            }

            ViewData["IdKorisnik"] = new SelectList(
                await _userManager.Users.Select(u => new { u.Id, Ime = u.Ime + " " + u.Prezime }).ToListAsync(),
                "Id", "Ime", karton.IdKorisnik);
            return View(karton);
        }

        [Authorize(Roles = Uloge.Kupac)]
        public async Task<IActionResult> MojKarton()
        {
            var user = await _userManager.GetUserAsync(User);
            var karton = await _context.ElektronskiKarton
                .FirstOrDefaultAsync(e => e.IdKorisnik == user!.Id);

            if (karton == null)
            {
                karton = new ElektronskiKarton
                {
                    IdKorisnik = user!.Id,
                    DatumKreiranja = DateTime.Now,
                    Napomena = "Automatski kreiran karton korisnika."
                };
                _context.ElektronskiKarton.Add(karton);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = karton.Id });
        }

        private async Task<bool> MozePristupitiAsync(ElektronskiKarton karton)
        {
            if (User.IsInRole(Uloge.Administrator) || User.IsInRole(Uloge.Zaposlenik))
                return true;

            var user = await _userManager.GetUserAsync(User);
            return user != null && karton.IdKorisnik == user.Id;
        }
    }
}
