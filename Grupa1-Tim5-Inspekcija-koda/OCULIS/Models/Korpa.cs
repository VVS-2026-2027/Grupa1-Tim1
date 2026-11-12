using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCULIS.Models
{
    public class Korpa
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Ukupna cijena")]
        public double UkupnaCijena { get; set; }

        [ForeignKey(nameof(Korisnik))]
        public string IdKorisnik { get; set; } = string.Empty;
        public Korisnik Korisnik { get; set; } = null!;

        public ICollection<StavkaKorpe> Stavke { get; set; } = new List<StavkaKorpe>();
    }
}
