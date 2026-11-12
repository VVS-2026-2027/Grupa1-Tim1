using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;
using OCULIS.Models.ViewModels;

namespace OCULIS.Controllers
{
    public class ProizvodController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProizvodController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index(ProizvodPretragaViewModel filter)
        {
            var query = _context.Proizvod.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Pretraga))
            {
                var pretraga = filter.Pretraga.ToLower();
                query = query.Where(p =>
                    p.Naziv.ToLower().Contains(pretraga) ||
                    p.Opis.ToLower().Contains(pretraga) ||
                    p.Proizvodjac.ToLower().Contains(pretraga));
            }

            if (filter.Kategorija.HasValue)
                query = query.Where(p => p.Kategorija == filter.Kategorija);

            if (!string.IsNullOrWhiteSpace(filter.Proizvodjac))
                query = query.Where(p => p.Proizvodjac.Contains(filter.Proizvodjac));

            if (filter.MinCijena.HasValue)
                query = query.Where(p => p.Cijena >= filter.MinCijena);

            if (filter.MaxCijena.HasValue)
                query = query.Where(p => p.Cijena <= filter.MaxCijena);

            query = filter.Sortiranje switch
            {
                "cijena_asc" => query.OrderBy(p => p.Cijena),
                "cijena_desc" => query.OrderByDescending(p => p.Cijena),
                "naziv" => query.OrderBy(p => p.Naziv),
                _ => query.OrderBy(p => p.Naziv)
            };

            filter.Proizvodi = await query.ToListAsync();
            return View(filter);
        }

        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var proizvod = await _context.Proizvod.FirstOrDefaultAsync(m => m.Id == id);
            if (proizvod == null) return NotFound();

            return View(proizvod);
        }

        [Authorize(Roles = Uloge.Administrator)]
        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Uloge.Administrator)]
        public async Task<IActionResult> Create([Bind("Naziv,Opis,Cijena,Kategorija,Proizvodjac,DostupnaKolicina,SlikaUrl")] Proizvod proizvod)
        {
            if (ModelState.IsValid)
            {
                _context.Add(proizvod);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Proizvod uspješno kreiran.";
                return RedirectToAction(nameof(Index));
            }
            return View(proizvod);
        }

        [Authorize(Roles = Uloge.Administrator)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var proizvod = await _context.Proizvod.FindAsync(id);
            if (proizvod == null) return NotFound();
            return View(proizvod);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Uloge.Administrator)]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,Opis,Cijena,Kategorija,Proizvodjac,DostupnaKolicina,SlikaUrl")] Proizvod proizvod)
        {
            if (id != proizvod.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(proizvod);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Proizvod uspješno ažuriran.";
                return RedirectToAction(nameof(Index));
            }
            return View(proizvod);
        }

        [Authorize(Roles = Uloge.Administrator)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var proizvod = await _context.Proizvod.FirstOrDefaultAsync(m => m.Id == id);
            if (proizvod == null) return NotFound();
            return View(proizvod);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = Uloge.Administrator)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proizvod = await _context.Proizvod.FindAsync(id);
            if (proizvod != null) _context.Proizvod.Remove(proizvod);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Proizvod obrisan.";
            return RedirectToAction(nameof(Index));
        }
    }
}
