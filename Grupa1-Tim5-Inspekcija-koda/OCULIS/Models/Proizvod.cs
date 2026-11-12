using System.ComponentModel.DataAnnotations;

namespace OCULIS.Models
{
    public class Proizvod
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Naziv proizvoda je obavezan.")]
        [StringLength(150)]
        [Display(Name = "Naziv")]
        public string Naziv { get; set; } = string.Empty;

        [Required(ErrorMessage = "Opis je obavezan.")]
        [StringLength(1000)]
        [Display(Name = "Opis")]
        public string Opis { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Cijena mora biti veća od 0.")]
        [Display(Name = "Cijena (KM)")]
        public double Cijena { get; set; }

        [Required]
        [Display(Name = "Kategorija")]
        public KategorijaProizvoda Kategorija { get; set; }

        [Required(ErrorMessage = "Proizvođač je obavezan.")]
        [StringLength(100)]
        [Display(Name = "Proizvođač")]
        public string Proizvodjac { get; set; } = string.Empty;

        [Range(0, int.MaxValue)]
        [Display(Name = "Dostupna količina")]
        public int DostupnaKolicina { get; set; }

        [Display(Name = "Slika URL")]
        public string? SlikaUrl { get; set; }
    }
}
