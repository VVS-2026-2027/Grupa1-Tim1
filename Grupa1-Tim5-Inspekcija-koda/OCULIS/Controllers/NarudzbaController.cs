using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Data;
using OCULIS.Models;
using OCULIS.Models.ViewModels;
using OCULIS.Services.Obavijest;
using OCULIS.Services.Popust;

namespace OCULIS.Controllers
{
    [Authorize]
    public class NarudzbaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Korisnik> _userManager;
        private readonly IPopustKalkulatorService _popustService;
        private readonly IObavijestServis _obavijestServis;

        public NarudzbaController(
            ApplicationDbContext context,
            UserManager<Korisnik> userManager,
            IPopustKalkulatorService popustService,
            IObavijestServis obavijestServis)
        {
            _context = context;
            _userManager = userManager;
            _popustService = popustService;
            _obavijestServis = obavijestServis;
        }

        public async Task<IActionResult> Index()
        {
            var query = _context.Narudzba
                .Include(n => n.Korisnik)
                .Include(n => n.Stavke)
                .ThenInclude(s => s.Proizvod)
                .AsQueryable();

            if (User.IsInRole(Uloge.Kupac))
            {
                var user = await _userManager.GetUserAsync(User);
                query = query.Where(n => n.IdKorisnik == user!.Id);
            }

            return View(await query.OrderByDescending(n => n.DatumNarudzbe).ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var narudzba = await _context.Narudzba
                .Include(n => n.Korisnik)
                .Include(n => n.Stavke)
                .ThenInclude(s => s.Proizvod)
                .Include(n => n.Placanja)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (narudzba == null) return NotFound();
            if (!await MozePristupitiAsync(narudzba)) return Forbid();

            return View(narudzba);
        }
        [Authorize(Roles = Uloge.Kupac)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Kreiraj(NarudzbaCheckoutViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);

            var korpa = await _context.Korpa
                .Include(k => k.Stavke)
                .ThenInclude(s => s.Proizvod)
                .FirstOrDefaultAsync(k => k.Id == model.KorpaId && k.IdKorisnik == user!.Id);

            if (korpa == null || !korpa.Stavke.Any())
            {
                TempData["Error"] = "Korpa je prazna.";
                return RedirectToAction("Index", "Korpa");
            }

            var popust = await _popustService.IzracunajPopustAsync(user!.Id, korpa.Id);

            if (!ModelState.IsValid)
            {
                model.Popust = popust;
                model.Korpa = new KorpaViewModel
                {
                    KorpaId = korpa.Id,
                    UkupnaCijena = korpa.Stavke.Sum(s => s.Cijena * s.Kolicina),
                    Popust = popust,
                    Stavke = korpa.Stavke.Select(s => new StavkaKorpeViewModel
                    {
                        Id = s.Id,
                        IdProizvod = s.IdProizvod,
                        NazivProizvoda = s.Proizvod.Naziv,
                        Kolicina = s.Kolicina,
                        Cijena = s.Cijena
                    }).ToList()
                };

                return View("~/Views/Korpa/Checkout.cshtml", model);
            }

            var punaAdresa =
                $"{model.AdresaIsporuke}, {model.Grad}, {model.PostanskiBroj}, Tel: {model.Telefon}";

            var narudzba = new Narudzba
            {
                DatumNarudzbe = DateTime.Now,
                Status = StatusNarudzbe.Zaprimljena,
                OsnovnaCijena = popust.OsnovnaCijena,
                PopustPostotak = popust.PopustPostotak,
                PopustIznos = popust.PopustIznos,
                UkupnaCijena = popust.UkupnaCijena,
                AdresaIsporuke = punaAdresa,
                IdKorisnik = user.Id,
                IdKorpa = korpa.Id
            };

            foreach (var stavka in korpa.Stavke)
            {
                narudzba.Stavke.Add(new StavkaNarudzbe
                {
                    IdProizvod = stavka.IdProizvod,
                    NazivProizvoda = stavka.Proizvod?.Naziv ?? "Proizvod",
                    Kolicina = stavka.Kolicina,
                    Cijena = stavka.Cijena
                });

                var proizvod = await _context.Proizvod.FindAsync(stavka.IdProizvod);

                if (proizvod != null)
                {
                    proizvod.DostupnaKolicina = Math.Max(0, proizvod.DostupnaKolicina - stavka.Kolicina);
                }
            }

            _context.Narudzba.Add(narudzba);
            _context.StavkaKorpe.RemoveRange(korpa.Stavke);
            korpa.UkupnaCijena = 0;

            user.BrojNarudzbi++;
            user.LojalnostBodovi += (int)(popust.UkupnaCijena / 10);

            await _userManager.UpdateAsync(user);
            await _context.SaveChangesAsync();
            await _obavijestServis.PosaljiObavijestNarudzbaAsync(user.Id, narudzba.Id);

            TempData["Success"] = $"Narudžba #{narudzba.Id} uspješno kreirana.";

            return RedirectToAction(nameof(Details), new { id = narudzba.Id });
        }

        [Authorize(Roles = $"{Uloge.Zaposlenik},{Uloge.Administrator}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var narudzba = await _context.Narudzba.FindAsync(id);
            if (narudzba == null) return NotFound();
            return View(narudzba);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = $"{Uloge.Zaposlenik},{Uloge.Administrator}")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Status")] Narudzba model)
        {
            var narudzba = await _context.Narudzba.FindAsync(id);
            if (narudzba == null) return NotFound();

            narudzba.Status = model.Status;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Status narudžbe ažuriran.";
            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<bool> MozePristupitiAsync(Narudzba narudzba)
        {
            if (User.IsInRole(Uloge.Administrator) || User.IsInRole(Uloge.Zaposlenik))
                return true;

            var user = await _userManager.GetUserAsync(User);
            return user != null && narudzba.IdKorisnik == user.Id;
        }
    }
}
