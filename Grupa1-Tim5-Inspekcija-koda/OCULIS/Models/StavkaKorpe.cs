using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCULIS.Models
{
    public class StavkaKorpe
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 100)]
        [Display(Name = "Količina")]
        public int Kolicina { get; set; }

        [Display(Name = "Cijena")]
        public double Cijena { get; set; }

        [ForeignKey(nameof(Proizvod))]
        public int IdProizvod { get; set; }
        public Proizvod Proizvod { get; set; } = null!;

        [ForeignKey(nameof(Korpa))]
        public int IdKorpa { get; set; }
        public Korpa Korpa { get; set; } = null!;
    }
}
