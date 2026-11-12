using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCULIS.Models
{
    public class Obavijest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [Display(Name = "Naslov")]
        public string Naslov { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        [Display(Name = "Tekst")]
        public string Tekst { get; set; } = string.Empty;

        [Display(Name = "Datum slanja")]
        public DateTime DatumSlanja { get; set; }

        [Display(Name = "Poslano emailom")]
        public bool PoslanoEmailom { get; set; }

        [ForeignKey(nameof(Korisnik))]
        public string IdKorisnik { get; set; } = string.Empty;
        public Korisnik Korisnik { get; set; } = null!;
    }
}
