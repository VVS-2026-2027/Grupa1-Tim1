using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCULIS.Models
{
    public class PregledVida
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Datum pregleda")]
        public DateTime DatumPregleda { get; set; }

        [Display(Name = "Dioptrija lijevo")]
        public double DioptrijaLijevo { get; set; }

        [Display(Name = "Dioptrija desno")]
        public double DioptrijaDesno { get; set; }

        [Required]
        [StringLength(500)]
        [Display(Name = "Preporuka")]
        public string Preporuka { get; set; } = string.Empty;

        [ForeignKey(nameof(ElektronskiKarton))]
        public int IdElektronskiKarton { get; set; }
        public ElektronskiKarton ElektronskiKarton { get; set; } = null!;
    }
}
