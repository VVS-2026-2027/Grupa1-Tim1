using System.ComponentModel.DataAnnotations;

namespace OCULIS.Models
{
    public class Poslovnica
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv poslovnice je obavezan.")]
        [StringLength(100)]
        [Display(Name = "Naziv")]
        public string Naziv { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adresa je obavezna.")]
        [StringLength(200)]
        [Display(Name = "Adresa")]
        public string Adresa { get; set; } = string.Empty;

        [Required(ErrorMessage = "Telefon je obavezan.")]
        [StringLength(20)]
        [Display(Name = "Telefon")]
        public string Telefon { get; set; } = string.Empty;

        [Required(ErrorMessage = "Radno vrijeme je obavezno.")]
        [StringLength(100)]
        [Display(Name = "Radno vrijeme")]
        public string RadnoVrijeme { get; set; } = string.Empty;

        [Display(Name = "Geografska širina")]
        public double Latitude { get; set; }

        [Display(Name = "Geografska dužina")]
        public double Longitude { get; set; }
    }
}
