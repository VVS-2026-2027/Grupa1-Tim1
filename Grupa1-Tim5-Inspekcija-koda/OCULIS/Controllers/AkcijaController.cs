using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;

namespace OCULIS.Controllers
{
    [Authorize(Roles = Uloge.Administrator)]
    public class AkcijaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AkcijaController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Akcija.Include(a => a.Proizvod).OrderByDescending(a => a.DatumPocetka).ToListAsync());
        }

        public async Task<IActionResult> Create()
        {
            ViewData["IdProizvod"] = new SelectList(await _context.Proizvod.ToListAsync(), "Id", "Naziv");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Naziv,PopustPostotak,DatumPocetka,DatumZavrsetka,Aktivna,IdProizvod")] Akcija akcija)
        {
            if (ModelState.IsValid)
            {
                _context.Add(akcija);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Akcija kreirana.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdProizvod"] = new SelectList(await _context.Proizvod.ToListAsync(), "Id", "Naziv", akcija.IdProizvod);
            return View(akcija);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var akcija = await _context.Akcija.FindAsync(id);
            if (akcija == null) return NotFound();
            ViewData["IdProizvod"] = new SelectList(await _context.Proizvod.ToListAsync(), "Id", "Naziv", akcija.IdProizvod);
            return View(akcija);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Naziv,PopustPostotak,DatumPocetka,DatumZavrsetka,Aktivna,IdProizvod")] Akcija akcija)
        {
            if (id != akcija.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(akcija);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Akcija ažurirana.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdProizvod"] = new SelectList(await _context.Proizvod.ToListAsync(), "Id", "Naziv", akcija.IdProizvod);
            return View(akcija);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var akcija = await _context.Akcija.Include(a => a.Proizvod).FirstOrDefaultAsync(a => a.Id == id);
            if (akcija == null) return NotFound();
            return View(akcija);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var akcija = await _context.Akcija.FindAsync(id);
            if (akcija != null) _context.Akcija.Remove(akcija);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
