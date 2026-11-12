using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;
using OCULIS.Models.ViewModels;

namespace OCULIS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<Korisnik> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var danas = DateTime.Today;

            ViewBag.UkupnoProizvoda = await _context.Proizvod.CountAsync();
            ViewBag.UkupnoPoslovnica = await _context.Poslovnica.CountAsync();

            ViewBag.IstaknutiProizvodi = await _context.Proizvod
                .Take(4)
                .ToListAsync();

            ViewBag.AktivneAkcije = await _context.Akcija
                .Include(a => a.Proizvod)
                .Where(a => a.Aktivna &&
                            a.DatumPocetka <= danas &&
                            a.DatumZavrsetka >= danas)
                .OrderByDescending(a => a.PopustPostotak)
                .Take(3)
                .ToListAsync();

            return View();
        }

        [Authorize(Roles = Uloge.Administrator)]
        public async Task<IActionResult> Dashboard()
        {
            var model = new DashboardViewModel
            {
                UkupnoProizvoda = await _context.Proizvod.CountAsync(),
                UkupnoNarudzbi = await _context.Narudzba.CountAsync(),
                UkupnoTermina = await _context.TerminPregleda.CountAsync(),
                UkupnoKorisnika = await _userManager.Users.CountAsync(),
                UkupniPrihod = await _context.Placanje
                    .Where(p => p.StatusPlacanja == StatusPlacanja.Uspjesno)
                    .SumAsync(p => p.Iznos),
                NedavneNarudzbe = await _context.Narudzba
                    .Include(n => n.Korisnik)
                    .OrderByDescending(n => n.DatumNarudzbe)
                    .Take(5)
                    .Select(n => new NarudzbaStatistika
                    {
                        Id = n.Id,
                        Korisnik = n.Korisnik.Ime + " " + n.Korisnik.Prezime,
                        Iznos = n.UkupnaCijena,
                        Status = n.Status.ToString(),
                        Datum = n.DatumNarudzbe
                    })
                    .ToListAsync(),
                NarudzbePoStatusu = await _context.Narudzba
                    .GroupBy(n => n.Status)
                    .Select(g => new { Status = g.Key.ToString(), Count = g.Count() })
                    .ToDictionaryAsync(x => x.Status, x => x.Count)
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
