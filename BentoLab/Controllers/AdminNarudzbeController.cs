using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BentoLab.Data;
using BentoLab.Models;

namespace BentoLab.Controllers
{
    public class AdminNarudzbeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminNarudzbeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. PRIKAZ SVIH NARUDŽBI SREĐEN PO ID-u (1, 2, 3...)
        public async Task<IActionResult> Index()
        {
            var sveNarudzbe = await _context.Narudzbe
                .OrderBy(n => n.NarudzbaID)
                .ToListAsync();

            return View(sveNarudzbe);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var narudzba = await _context.Narudzbe
            .Include(n => n.Korisnik)
            .Include(n => n.Stavke)
            .ThenInclude(s => s.Torta)
            .FirstOrDefaultAsync(n => n.NarudzbaID == id);

            if (narudzba == null)
                return NotFound();

            return View(narudzba);
        }
        // 2. AKCIJA ZA PROMJENU STATUSA NARUDŽBE (U bazi i na ekranu)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PromijeniStatus(int id, string noviStatus)
        {
            var narudzba = await _context.Narudzbe.FindAsync(id);

            if (narudzba != null)
            {
                // Pretvaramo tekst iz padajućeg menija u Enum (KREIRANA, POTVRDJENA...)
                if (Enum.TryParse(typeof(StatusNarudzbe), noviStatus, out var statusEnum))
                {
                    narudzba.Status = (StatusNarudzbe)statusEnum;
                    _context.Update(narudzba);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }
    }
}