using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;

namespace OCULIS.Controllers
{
    public class PoslovnicaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PoslovnicaController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            ViewBag.GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"] ?? "";
            return View(await _context.Poslovnica.ToListAsync());
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var poslovnica = await _context.Poslovnica.FirstOrDefaultAsync(m => m.Id == id);
            if (poslovnica == null) return NotFound();

            ViewBag.GoogleMapsApiKey = _configuration["GoogleMaps:ApiKey"] ?? "";
            return View(poslovnica);
        }

        [Authorize(Roles = Uloge.Administrator)]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Uloge.Administrator)]
        public async Task<IActionResult> Create([Bind("Naziv,Adresa,Telefon,RadnoVrijeme,Latitude,Longitude")] Poslovnica poslovnica)
        {
            if (ModelState.IsValid)
            {
                _context.Add(poslovnica);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Poslovnica kreirana.";
                return RedirectToAction(nameof(Index));
            }
            return View(poslovnica);
        }

        [Authorize(Roles = Uloge.Administrator)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var poslovnica = await _context.Poslovnica.FindAsync(id);
            if (poslovnica == null) return NotFound();
            return View(poslovnica);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Uloge.Administrator)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,Adresa,Telefon,RadnoVrijeme,Latitude,Longitude")] Poslovnica poslovnica)
        {
            if (id != poslovnica.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(poslovnica);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Poslovnica ažurirana.";
                return RedirectToAction(nameof(Index));
            }
            return View(poslovnica);
        }

        [Authorize(Roles = Uloge.Administrator)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var poslovnica = await _context.Poslovnica.FirstOrDefaultAsync(m => m.Id == id);
            if (poslovnica == null) return NotFound();
            return View(poslovnica);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Uloge.Administrator)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var poslovnica = await _context.Poslovnica.FindAsync(id);
            if (poslovnica != null) _context.Poslovnica.Remove(poslovnica);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
