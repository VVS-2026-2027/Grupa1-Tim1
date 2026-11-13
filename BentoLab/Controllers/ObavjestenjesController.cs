using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BentoLab.Data;
using BentoLab.Models;

namespace BentoLab.Controllers
{
    public class ObavjestenjesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ObavjestenjesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Obavjestenjes
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Obavjestenja.Include(o => o.Korisnik).Include(o => o.Narudzba);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Obavjestenjes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obavjestenje = await _context.Obavjestenja
                .Include(o => o.Korisnik)
                .Include(o => o.Narudzba)
                .FirstOrDefaultAsync(m => m.ObavjestenjeID == id);
            if (obavjestenje == null)
            {
                return NotFound();
            }

            return View(obavjestenje);
        }

        // GET: Obavjestenjes/Create
        public IActionResult Create()
        {
            ViewData["KorisnikID"] = new SelectList(_context.Korisnik, "KorisnikID", "KorisnikID");
            ViewData["NarudzbaID"] = new SelectList(_context.Narudzbe, "NarudzbaID", "NarudzbaID");
            return View();
        }

        // POST: Obavjestenjes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ObavjestenjeID,Sadrzaj,DatumSlanja,KorisnikID,NarudzbaID")] Obavjestenje obavjestenje)
        {
            if (ModelState.IsValid)
            {
                _context.Add(obavjestenje);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["KorisnikID"] = new SelectList(_context.Korisnik, "KorisnikID", "KorisnikID", obavjestenje.KorisnikID);
            ViewData["NarudzbaID"] = new SelectList(_context.Narudzbe, "NarudzbaID", "NarudzbaID", obavjestenje.NarudzbaID);
            return View(obavjestenje);
        }

        // GET: Obavjestenjes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obavjestenje = await _context.Obavjestenja.FindAsync(id);
            if (obavjestenje == null)
            {
                return NotFound();
            }
            ViewData["KorisnikID"] = new SelectList(_context.Korisnik, "KorisnikID", "KorisnikID", obavjestenje.KorisnikID);
            ViewData["NarudzbaID"] = new SelectList(_context.Narudzbe, "NarudzbaID", "NarudzbaID", obavjestenje.NarudzbaID);
            return View(obavjestenje);
        }

        // POST: Obavjestenjes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ObavjestenjeID,Sadrzaj,DatumSlanja,KorisnikID,NarudzbaID")] Obavjestenje obavjestenje)
        {
            if (id != obavjestenje.ObavjestenjeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(obavjestenje);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ObavjestenjeExists(obavjestenje.ObavjestenjeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["KorisnikID"] = new SelectList(_context.Korisnik, "KorisnikID", "KorisnikID", obavjestenje.KorisnikID);
            ViewData["NarudzbaID"] = new SelectList(_context.Narudzbe, "NarudzbaID", "NarudzbaID", obavjestenje.NarudzbaID);
            return View(obavjestenje);
        }

        // GET: Obavjestenjes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var obavjestenje = await _context.Obavjestenja
                .Include(o => o.Korisnik)
                .Include(o => o.Narudzba)
                .FirstOrDefaultAsync(m => m.ObavjestenjeID == id);
            if (obavjestenje == null)
            {
                return NotFound();
            }

            return View(obavjestenje);
        }

        // POST: Obavjestenjes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var obavjestenje = await _context.Obavjestenja.FindAsync(id);
            if (obavjestenje != null)
            {
                _context.Obavjestenja.Remove(obavjestenje);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ObavjestenjeExists(int id)
        {
            return _context.Obavjestenja.Any(e => e.ObavjestenjeID == id);
        }
    }
}
