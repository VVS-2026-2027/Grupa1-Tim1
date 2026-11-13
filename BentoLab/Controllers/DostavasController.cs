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
    public class DostavasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DostavasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Dostavas
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Dostave.Include(d => d.Narudzba);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Dostavas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dostava = await _context.Dostave
                .Include(d => d.Narudzba)
                .FirstOrDefaultAsync(m => m.DostavaID == id);
            if (dostava == null)
            {
                return NotFound();
            }

            return View(dostava);
        }

        // GET: Dostavas/Create
        public IActionResult Create()
        {
            ViewData["NarudzbaID"] = new SelectList(_context.Narudzbe, "NarudzbaID", "NarudzbaID");
            return View();
        }

        // POST: Dostavas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DostavaID,Adresa,KontaktTelefon,CijenaDostave,VrijemeIsporuke,Napomena,NarudzbaID")] Dostava dostava)
        {
            if (ModelState.IsValid)
            {
                _context.Add(dostava);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NarudzbaID"] = new SelectList(_context.Narudzbe, "NarudzbaID", "NarudzbaID", dostava.NarudzbaID);
            return View(dostava);
        }

        // GET: Dostavas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dostava = await _context.Dostave.FindAsync(id);
            if (dostava == null)
            {
                return NotFound();
            }
            ViewData["NarudzbaID"] = new SelectList(_context.Narudzbe, "NarudzbaID", "NarudzbaID", dostava.NarudzbaID);
            return View(dostava);
        }

        // POST: Dostavas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DostavaID,Adresa,KontaktTelefon,CijenaDostave,VrijemeIsporuke,Napomena,NarudzbaID")] Dostava dostava)
        {
            if (id != dostava.DostavaID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(dostava);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DostavaExists(dostava.DostavaID))
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
            ViewData["NarudzbaID"] = new SelectList(_context.Narudzbe, "NarudzbaID", "NarudzbaID", dostava.NarudzbaID);
            return View(dostava);
        }

        // GET: Dostavas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var dostava = await _context.Dostave
                .Include(d => d.Narudzba)
                .FirstOrDefaultAsync(m => m.DostavaID == id);
            if (dostava == null)
            {
                return NotFound();
            }

            return View(dostava);
        }

        // POST: Dostavas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var dostava = await _context.Dostave.FindAsync(id);
            if (dostava != null)
            {
                _context.Dostave.Remove(dostava);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DostavaExists(int id)
        {
            return _context.Dostave.Any(e => e.DostavaID == id);
        }
    }
}
