using System.ComponentModel.DataAnnotations;

namespace OCULIS.Models.ViewModels
{
    public class NarudzbaCheckoutViewModel
    {
        public int KorpaId { get; set; }

        [Required(ErrorMessage = "Adresa isporuke je obavezna.")]
        [StringLength(150, MinimumLength = 5, ErrorMessage = "Adresa mora imati između 5 i 150 karaktera.")]
        [RegularExpression(@"^(?=.*[A-Za-zČĆŽŠĐčćžšđ])(?=.*(\d|bb|BB))[A-Za-zČĆŽŠĐčćžšđ0-9\s\.\-\/,]+$",
            ErrorMessage = "Adresa mora sadržavati naziv ulice i broj ili oznaku bb.")]
        [Display(Name = "Adresa")]
        public string AdresaIsporuke { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grad je obavezan.")]
        [StringLength(40, MinimumLength = 2, ErrorMessage = "Grad mora imati između 2 i 40 karaktera.")]
        [RegularExpression(@"^[A-Za-zČĆŽŠĐčćžšđ\s-]+$",
            ErrorMessage = "Grad smije sadržavati samo slova.")]
        [Display(Name = "Grad")]
        public string Grad { get; set; } = string.Empty;

        [Required(ErrorMessage = "Poštanski broj je obavezan.")]
        [RegularExpression(@"^\d{5}$",
            ErrorMessage = "Poštanski broj mora imati tačno 5 cifara.")]
        [Display(Name = "Poštanski broj")]
        public string PostanskiBroj { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon je obavezan.")]
        [RegularExpression(@"^0\d{2}\s?\d{3}\s?\d{3}$",
            ErrorMessage = "Telefon mora biti u formatu 061 234 567.")]
        [Display(Name = "Telefon")]
        public string Telefon { get; set; } = string.Empty;

        public PopustRezultatViewModel Popust { get; set; } = new();

        public KorpaViewModel Korpa { get; set; } = new();
    }
}