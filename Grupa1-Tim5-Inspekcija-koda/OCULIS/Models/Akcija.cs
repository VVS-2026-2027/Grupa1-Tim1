using System.ComponentModel.DataAnnotations;

namespace OCULIS.Models
{
    public class Akcija
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Naziv akcije")]
        public string Naziv { get; set; } = string.Empty;

        [Range(0, 100)]
        [Display(Name = "Popust (%)")]
        public double PopustPostotak { get; set; }

        [Display(Name = "Datum početka")]
        public DateTime DatumPocetka { get; set; }

        [Display(Name = "Datum završetka")]
        public DateTime DatumZavrsetka { get; set; }

        [Display(Name = "Aktivna")]
        public bool Aktivna { get; set; } = true;

        public int? IdProizvod { get; set; }
        public Proizvod? Proizvod { get; set; }
    }
}
