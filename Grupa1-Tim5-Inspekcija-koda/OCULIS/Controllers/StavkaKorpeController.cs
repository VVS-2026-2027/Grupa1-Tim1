using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OCULIS.Data;
using OCULIS.Models;

using Microsoft.AspNetCore.Authorization;
using OCULIS.Constants;

namespace OCULIS.Controllers
{
    [Authorize(Roles = Uloge.Administrator)]
    public class StavkaKorpeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StavkaKorpeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: StavkaKorpe
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.StavkaKorpe.Include(s => s.Korpa).Include(s => s.Proizvod);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: StavkaKorpe/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stavkaKorpe = await _context.StavkaKorpe
                .Include(s => s.Korpa)
                .Include(s => s.Proizvod)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stavkaKorpe == null)
            {
                return NotFound();
            }

            return View(stavkaKorpe);
        }

        // GET: StavkaKorpe/Create
        public IActionResult Create()
        {
            ViewData["IdKorpa"] = new SelectList(_context.Korpa, "Id", "Id");
            ViewData["IdProizvod"] = new SelectList(_context.Proizvod, "Id", "Id");
            return View();
        }

        // POST: StavkaKorpe/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Kolicina,Cijena,IdProizvod,IdKorpa")] StavkaKorpe stavkaKorpe)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stavkaKorpe);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdKorpa"] = new SelectList(_context.Korpa, "Id", "Id", stavkaKorpe.IdKorpa);
            ViewData["IdProizvod"] = new SelectList(_context.Proizvod, "Id", "Id", stavkaKorpe.IdProizvod);
            return View(stavkaKorpe);
        }

        // GET: StavkaKorpe/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stavkaKorpe = await _context.StavkaKorpe.FindAsync(id);
            if (stavkaKorpe == null)
            {
                return NotFound();
            }
            ViewData["IdKorpa"] = new SelectList(_context.Korpa, "Id", "Id", stavkaKorpe.IdKorpa);
            ViewData["IdProizvod"] = new SelectList(_context.Proizvod, "Id", "Id", stavkaKorpe.IdProizvod);
            return View(stavkaKorpe);
        }

        // POST: StavkaKorpe/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Kolicina,Cijena,IdProizvod,IdKorpa")] StavkaKorpe stavkaKorpe)
        {
            if (id != stavkaKorpe.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stavkaKorpe);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StavkaKorpeExists(stavkaKorpe.Id))
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
            ViewData["IdKorpa"] = new SelectList(_context.Korpa, "Id", "Id", stavkaKorpe.IdKorpa);
            ViewData["IdProizvod"] = new SelectList(_context.Proizvod, "Id", "Id", stavkaKorpe.IdProizvod);
            return View(stavkaKorpe);
        }

        // GET: StavkaKorpe/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stavkaKorpe = await _context.StavkaKorpe
                .Include(s => s.Korpa)
                .Include(s => s.Proizvod)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (stavkaKorpe == null)
            {
                return NotFound();
            }

            return View(stavkaKorpe);
        }

        // POST: StavkaKorpe/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stavkaKorpe = await _context.StavkaKorpe.FindAsync(id);
            if (stavkaKorpe != null)
            {
                _context.StavkaKorpe.Remove(stavkaKorpe);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StavkaKorpeExists(int id)
        {
            return _context.StavkaKorpe.Any(e => e.Id == id);
        }
    }
}
