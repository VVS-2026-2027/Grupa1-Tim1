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
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Korisnik> _signInManager;
        private readonly UserManager<Korisnik> _userManager;

        public RegisterModel(UserManager<Korisnik> userManager, SignInManager<Korisnik> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Ime je obavezno.")]
            public string Ime { get; set; }

            [Required(ErrorMessage = "Prezime je obavezno.")]
            public string Prezime { get; set; }

            [Required(ErrorMessage = "Email je obavezan.")]
            [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Lozinka je obavezna.")]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Lozinke se ne podudaraju.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                // Kreiramo novog korisnika i spajamo Ime i Prezime u jedno polje
                var user = new Korisnik
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    ImePrezime = $"{Input.Ime} {Input.Prezime}"
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    // Preusmjeravanje na RegisterConfirmation stranicu sa email parametrom
                    return RedirectToPage("RegisterConfirmation", new { email = Input.Email });
                }

                // Ako kreiranje nije uspjelo, prevodimo greške sistema na bosanski jezik
                foreach (var error in result.Errors)
                {
                    string prevedenaGreska = error.Description;

                    if (error.Code == "PasswordRequiresNonAlphanumeric")
                    {
                        prevedenaGreska = "Lozinka mora sadržavati najmanje jedan specijalni karakter (npr. !, @, #, _, *).";
                    }
                    else if (error.Code == "PasswordRequiresUpper")
                    {
                        prevedenaGreska = "Lozinka mora sadržavati najmanje jedno veliko slovo ('A'-'Z').";
                    }
                    else if (error.Code == "PasswordRequiresLower")
                    {
                        prevedenaGreska = "Lozinka mora sadržavati najmanje jedno malo slovo ('a'-'z').";
                    }
                    else if (error.Code == "PasswordRequiresDigit")
                    {
                        prevedenaGreska = "Lozinka mora sadržavati najmanje jedan broj ('0'-'9').";
                    }
                    else if (error.Code == "PasswordTooShort")
                    {
                        prevedenaGreska = "Lozinka je prekratka.";
                    }
                    else if (error.Code == "DuplicateUserName" || error.Code == "DuplicateEmail")
                    {
                        prevedenaGreska = "Korisnik sa ovim emailom već postoji.";
                    }

                    ModelState.AddModelError(string.Empty, prevedenaGreska);
                }
            }

            // Ako validacija forme nije prošla, ponovo učitavamo stranicu sa greškama
            return Page();
        }
    }
}