using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using BentoLab.Models;

namespace BentoLab.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;

        public HomeController(UserManager<Korisnik> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public async Task<IActionResult> AdminPanel()
        {
            var trenutniKorisnik = await _userManager.GetUserAsync(User);

            if (trenutniKorisnik == null || trenutniKorisnik.Uloga != Uloga.ADMIN)
                return RedirectToAction("Index", "Torta");

            return View();
        }

        [Authorize]
        public async Task<IActionResult> UpravljanjeKorisnicima()
        {
            var trenutniKorisnik = await _userManager.GetUserAsync(User);

            if (trenutniKorisnik == null || trenutniKorisnik.Uloga != Uloga.ADMIN)
                return RedirectToAction("Index", "Torta");

            var korisnici = _userManager.Users
            .Where(k => k.Uloga == Uloga.KUPAC)
            .OrderByDescending(k => k.Id)
            .ToList();

            return View(korisnici);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> BlokirajKorisnika(int id)
        {
            var admin = await _userManager.GetUserAsync(User);

            if (admin == null || admin.Uloga != Uloga.ADMIN)
                return RedirectToAction("Index", "Torta");

            var korisnik = await _userManager.FindByIdAsync(id.ToString());

            if (korisnik != null && korisnik.Uloga == Uloga.KUPAC)
            {
                await _userManager.SetLockoutEnabledAsync(korisnik, true);
                await _userManager.SetLockoutEndDateAsync(korisnik, DateTimeOffset.UtcNow.AddYears(100));
            }

            return RedirectToAction(nameof(UpravljanjeKorisnicima));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> OdblokirajKorisnika(int id)
        {
            var admin = await _userManager.GetUserAsync(User);

            if (admin == null || admin.Uloga != Uloga.ADMIN)
                return RedirectToAction("Index", "Torta");

            var korisnik = await _userManager.FindByIdAsync(id.ToString());

            if (korisnik != null && korisnik.Uloga == Uloga.KUPAC)
            {
                await _userManager.SetLockoutEndDateAsync(korisnik, null);
            }

            return RedirectToAction(nameof(UpravljanjeKorisnicima));
        }


    }
}
