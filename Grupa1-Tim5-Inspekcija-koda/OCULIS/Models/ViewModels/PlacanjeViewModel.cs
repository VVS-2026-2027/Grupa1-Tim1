using System.ComponentModel.DataAnnotations;
using OCULIS.Models;

namespace OCULIS.Models.ViewModels
{
    public class PlacanjeViewModel
    {
        public int IdNarudzba { get; set; }
        public double Iznos { get; set; }
        public string StatusNarudzbe { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Način plaćanja")]
        public NacinPlacanja NacinPlacanja { get; set; }

        [Display(Name = "Broj kartice")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "Broj kartice mora sadržavati tačno 16 cifara.")]
        public string? BrojKartice { get; set; }

        [Display(Name = "Datum isteka")]
        [RegularExpression(@"^(0[1-9]|1[0-2])\/\d{2}$", ErrorMessage = "Datum isteka mora biti u formatu MM/YY.")]
        public string? DatumIsteka { get; set; }

        [Display(Name = "CVV")]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "CVV mora sadržavati tačno 3 cifre.")]
        public string? CVV { get; set; }
    }
}
