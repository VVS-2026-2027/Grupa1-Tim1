using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using BentoLab.Models;

namespace BentoLab.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<Korisnik> _signInManager;
        private readonly UserManager<Korisnik> _userManager;

        public LoginModel(SignInManager<Korisnik> signInManager, UserManager<Korisnik> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email je obavezan.")]
            [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Lozinka je obavezna.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            // Ako je returnUrl prazan, stavljamo da default bude početna stranica za Torte
            returnUrl ??= Url.Action("Index", "Torta");

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Korisnik sa ovim emailom ne postoji.");
                    return Page();
                }

                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError(string.Empty, "Račun nije verifikovan. Molimo potvrdite vaš email.");
                    return Page();
                }

                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var adminKorisnik = await _userManager.FindByEmailAsync(Input.Email);

                    // 1. Ako je u pitanju Admin, njega UVIJEK šaljemo u AdminPanel bez obzira na sve
                    if (adminKorisnik != null && adminKorisnik.Uloga == Uloga.ADMIN)
                    {
                        return RedirectToAction("AdminPanel", "Home");
                    }

                    // 2. Ako je obični kupac, vraćamo ga tamo odakle je došao (npr. u korpu)
                    // Radimo LocalRedirect samo ako je URL siguran i lokalni
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return LocalRedirect(returnUrl);
                    }

                    return RedirectToAction("Index", "Torta");
                }

                if (result.IsLockedOut)
                {
                    ModelState.AddModelError(string.Empty, "Ovaj račun je zaključan.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Pogrešna lozinka. Pokušajte ponovo.");
                }
            }
            return Page();
        }
    }
}