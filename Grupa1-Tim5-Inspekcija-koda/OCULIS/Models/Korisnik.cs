using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCULIS.Models
{
    public class Korisnik : IdentityUser
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        [StringLength(50)]
        [Display(Name = "Ime")]
        public string Ime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [StringLength(50)]
        [Display(Name = "Prezime")]
        public string Prezime { get; set; } = string.Empty;

        [Display(Name = "Telefon")]
        public string? Telefon { get; set; }

        [Display(Name = "Broj narudžbi")]
        public int BrojNarudzbi { get; set; }

        [Display(Name = "Lojalnost bodovi")]
        public int LojalnostBodovi { get; set; }

        [NotMapped]
        [Display(Name = "Puno ime")]
        public string PunoIme => $"{Ime} {Prezime}";
    }
}
