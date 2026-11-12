using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCULIS.Models
{
    public class Reklamacija
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Opis reklamacije je obavezan.")]
        [StringLength(1000)]
        [Display(Name = "Opis")]
        public string Opis { get; set; } = string.Empty;

        [Display(Name = "Datum podnošenja")]
        public DateTime DatumPodnosenja { get; set; }

        [Display(Name = "Status")]
        public StatusReklamacije Status { get; set; }

        [StringLength(500)]
        [Display(Name = "Odgovor")]
        public string? Odgovor { get; set; }

        [ForeignKey(nameof(Korisnik))]
        public string IdKorisnik { get; set; } = string.Empty;
        public Korisnik Korisnik { get; set; } = null!;

        [ForeignKey(nameof(Narudzba))]
        public int? IdNarudzba { get; set; }
        public Narudzba? Narudzba { get; set; }

        [ForeignKey(nameof(ElektronskiKarton))]
        public int? IdElektronskiKarton { get; set; }
        public ElektronskiKarton? ElektronskiKarton { get; set; }
    }
}
