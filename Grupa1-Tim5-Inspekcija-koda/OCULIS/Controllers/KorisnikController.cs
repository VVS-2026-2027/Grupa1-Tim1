using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OCULIS.Constants;
using OCULIS.Models;

namespace OCULIS.Controllers
{
    [Authorize(Roles = Uloge.Administrator)]
    public class KorisnikController : Controller
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public KorisnikController(UserManager<Korisnik> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var korisnici = await _userManager.Users.ToListAsync();
            ViewBag.Uloge = new Dictionary<string, IList<string>>();

            foreach (var k in korisnici)
                ViewBag.Uloge[k.Id] = await _userManager.GetRolesAsync(k);

            return View(korisnici);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();

            var korisnik = await _userManager.FindByIdAsync(id);
            if (korisnik == null) return NotFound();

            ViewBag.Uloge = await _userManager.GetRolesAsync(korisnik);
            return View(korisnik);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Korisnik korisnik, string password, string uloga)
        {
            if (ModelState.IsValid)
            {
                korisnik.UserName = korisnik.Email;
                korisnik.EmailConfirmed = true;
                var result = await _userManager.CreateAsync(korisnik, password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(uloga) && await _roleManager.RoleExistsAsync(uloga))
                        await _userManager.AddToRoleAsync(korisnik, uloga);

                    TempData["Success"] = "Korisnik kreiran.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            ViewBag.Uloge = Uloge.Sve;
            return View(korisnik);
        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null) return NotFound();

            var korisnik = await _userManager.FindByIdAsync(id);
            if (korisnik == null) return NotFound();

            ViewBag.Uloge = Uloge.Sve;
            ViewBag.TrenutnaUloga = (await _userManager.GetRolesAsync(korisnik)).FirstOrDefault();
            return View(korisnik);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Korisnik korisnik, string uloga)
        {
            if (id != korisnik.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existing = await _userManager.FindByIdAsync(id);
                if (existing == null) return NotFound();

                existing.Ime = korisnik.Ime;
                existing.Prezime = korisnik.Prezime;
                existing.Email = korisnik.Email;
                existing.UserName = korisnik.Email;
                existing.PhoneNumber = korisnik.PhoneNumber;
                existing.Telefon = korisnik.Telefon;

                var result = await _userManager.UpdateAsync(existing);
                if (result.Succeeded)
                {
                    var trenutne = await _userManager.GetRolesAsync(existing);
                    await _userManager.RemoveFromRolesAsync(existing, trenutne);
                    if (!string.IsNullOrEmpty(uloga))
                        await _userManager.AddToRoleAsync(existing, uloga);

                    TempData["Success"] = "Korisnik ažuriran.";
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }

            ViewBag.Uloge = Uloge.Sve;
            return View(korisnik);
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null) return NotFound();
            var korisnik = await _userManager.FindByIdAsync(id);
            if (korisnik == null) return NotFound();
            return View(korisnik);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var korisnik = await _userManager.FindByIdAsync(id);
            if (korisnik != null) await _userManager.DeleteAsync(korisnik);
            return RedirectToAction(nameof(Index));
        }
    }
}
