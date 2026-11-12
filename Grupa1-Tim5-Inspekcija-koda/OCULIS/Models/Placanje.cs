using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCULIS.Models
{
    public class Placanje
    {
        [Key]
        public int Id { get; set; }

        [Range(0.01, double.MaxValue)]
        [Display(Name = "Iznos")]
        public double Iznos { get; set; }

        [Display(Name = "Status plaćanja")]
        public StatusPlacanja StatusPlacanja { get; set; }

        [Display(Name = "Način plaćanja")]
        public NacinPlacanja NacinPlacanja { get; set; }

        [Display(Name = "Datum plaćanja")]
        public DateTime DatumPlacanja { get; set; }

        [StringLength(50)]
        [Display(Name = "Referenca transakcije")]
        public string? ReferencaTransakcije { get; set; }

        [ForeignKey(nameof(Narudzba))]
        public int IdNarudzba { get; set; }
        public Narudzba Narudzba { get; set; } = null!;
    }
}
