using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCULIS.Models
{
    public class ElektronskiKarton
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Datum kreiranja")]
        public DateTime DatumKreiranja { get; set; }

        [StringLength(500)]
        [Display(Name = "Napomena")]
        public string Napomena { get; set; } = string.Empty;

        [ForeignKey(nameof(Korisnik))]
        public string IdKorisnik { get; set; } = string.Empty;
        public Korisnik Korisnik { get; set; } = null!;

        public ICollection<PregledVida> Pregledi { get; set; } = new List<PregledVida>();
        public ICollection<Reklamacija> Reklamacije { get; set; } = new List<Reklamacija>();
    }
}
