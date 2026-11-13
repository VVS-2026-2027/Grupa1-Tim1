using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BentoLab.Data;
using BentoLab.Models;

namespace BentoLab.Controllers
{
    public class StavkaNarudzbesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StavkaNarudzbesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var stavke = _context.StavkeNarudzbe
            .Include(s => s.Narudzba)
            .Include(s => s.Torta);

            return View(await stavke.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var stavkaNarudzbe = await _context.StavkeNarudzbe
            .Include(s => s.Narudzba)
            .Include(s => s.Torta)
            .FirstOrDefaultAsync(m => m.StavkaID == id);

            if (stavkaNarudzbe == null) return NotFound();

            return View(stavkaNarudzbe);
        }

        public IActionResult Create()
        {
            ViewBag.TortaID = new SelectList(_context.Torta, "TortaID", "Naziv");
            return View(new StavkaNarudzbe { Kolicina = 1 });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StavkaID,Kolicina,TortaID")] StavkaNarudzbe stavkaNarudzbe)
        {
            ModelState.Remove("Narudzba");
            ModelState.Remove("Torta");

            var narudzba = await _context.Narudzbe
            .OrderByDescending(n => n.NarudzbaID)
            .FirstOrDefaultAsync();

            if (narudzba == null)
                return RedirectToAction("Create", "Narudzbas");

            stavkaNarudzbe.NarudzbaID = narudzba.NarudzbaID;

            var torta = await _context.Torta.FindAsync(stavkaNarudzbe.TortaID);

            if (torta == null)
                return NotFound();

            stavkaNarudzbe.CijenaStavke = torta.Cijena;

            if (ModelState.IsValid)
            {
                _context.Add(stavkaNarudzbe);
                await _context.SaveChangesAsync();

                narudzba.UkupnaCijena = await _context.StavkeNarudzbe
                .Where(s => s.NarudzbaID == narudzba.NarudzbaID)
                .SumAsync(s => s.Kolicina * s.CijenaStavke);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.TortaID = new SelectList(_context.Torta, "TortaID", "Naziv", stavkaNarudzbe.TortaID);
            return View(stavkaNarudzbe);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var stavkaNarudzbe = await _context.StavkeNarudzbe.FindAsync(id);
            if (stavkaNarudzbe == null) return NotFound();

            ViewBag.TortaID = new SelectList(_context.Torta, "TortaID", "Naziv", stavkaNarudzbe.TortaID);
            return View(stavkaNarudzbe);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StavkaID,Kolicina,NarudzbaID,TortaID")] StavkaNarudzbe stavkaNarudzbe)
        {
            if (id != stavkaNarudzbe.StavkaID) return NotFound();

            ModelState.Remove("Narudzba");
            ModelState.Remove("Torta");

            var torta = await _context.Torta.FindAsync(stavkaNarudzbe.TortaID);
            if (torta == null) return NotFound();

            stavkaNarudzbe.CijenaStavke = torta.Cijena;

            if (ModelState.IsValid)
            {
                _context.Update(stavkaNarudzbe);
                await _context.SaveChangesAsync();

                var narudzba = await _context.Narudzbe.FindAsync(stavkaNarudzbe.NarudzbaID);
                if (narudzba != null)
                {
                    narudzba.UkupnaCijena = await _context.StavkeNarudzbe
                    .Where(s => s.NarudzbaID == narudzba.NarudzbaID)
                    .SumAsync(s => s.Kolicina * s.CijenaStavke);

                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            ViewBag.TortaID = new SelectList(_context.Torta, "TortaID", "Naziv", stavkaNarudzbe.TortaID);
            return View(stavkaNarudzbe);
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var stavkaNarudzbe = await _context.StavkeNarudzbe
            .Include(s => s.Narudzba)
            .Include(s => s.Torta)
            .FirstOrDefaultAsync(m => m.StavkaID == id);

            if (stavkaNarudzbe == null) return NotFound();

            return View(stavkaNarudzbe);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stavkaNarudzbe = await _context.StavkeNarudzbe.FindAsync(id);

            if (stavkaNarudzbe != null)
            {
                int narudzbaId = stavkaNarudzbe.NarudzbaID;

                _context.StavkeNarudzbe.Remove(stavkaNarudzbe);
                await _context.SaveChangesAsync();

                var narudzba = await _context.Narudzbe.FindAsync(narudzbaId);
                if (narudzba != null)
                {
                    narudzba.UkupnaCijena = await _context.StavkeNarudzbe
                    .Where(s => s.NarudzbaID == narudzbaId)
                    .SumAsync(s => s.Kolicina * s.CijenaStavke);

                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StavkaNarudzbeExists(int id)
        {
            return _context.StavkeNarudzbe.Any(e => e.StavkaID == id);
        }
    }
}
