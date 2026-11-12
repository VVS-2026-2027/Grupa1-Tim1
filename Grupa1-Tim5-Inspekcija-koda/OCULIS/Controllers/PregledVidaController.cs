using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;

namespace OCULIS.Controllers
{
    [Authorize(Roles = $"{Uloge.Zaposlenik},{Uloge.Administrator}")]
    public class PregledVidaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PregledVidaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.PregledVida
                .Include(p => p.ElektronskiKarton)
                .ThenInclude(e => e.Korisnik)
                .OrderByDescending(p => p.DatumPregleda)
                .ToListAsync());
        }

        public async Task<IActionResult> Create(int? kartonId)
        {
            await PopuniKartoneAsync(kartonId);

            var pregled = new PregledVida
            {
                DatumPregleda = DateTime.Today,
                IdElektronskiKarton = kartonId ?? 0
            };

            return View(pregled);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DatumPregleda,DioptrijaLijevo,DioptrijaDesno,Preporuka,IdElektronskiKarton")] PregledVida pregled)
        {
            ModelState.Remove(nameof(PregledVida.ElektronskiKarton));

            if (pregled.IdElektronskiKarton == 0)
            {
                ModelState.AddModelError(nameof(PregledVida.IdElektronskiKarton), "Molimo odaberite elektronski karton.");
            }

            if (!ModelState.IsValid)
            {
                await PopuniKartoneAsync(pregled.IdElektronskiKarton);
                return View(pregled);
            }

            _context.PregledVida.Add(pregled);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Pregled vida je uspješno evidentiran.";

            return RedirectToAction("Details", "ElektronskiKarton", new { id = pregled.IdElektronskiKarton });
        }

        private async Task PopuniKartoneAsync(int? odabraniKarton = null)
        {
            var kartoni = await _context.ElektronskiKarton
                .Include(e => e.Korisnik)
                .OrderBy(e => e.Id)
                .Select(e => new
                {
                    e.Id,
                    Naziv = e.Korisnik != null
                        ? e.Korisnik.Ime + " " + e.Korisnik.Prezime + " - Karton #" + e.Id
                        : "Karton #" + e.Id
                })
                .ToListAsync();

            ViewData["IdElektronskiKarton"] = new SelectList(kartoni, "Id", "Naziv", odabraniKarton);
        }
    }
} 