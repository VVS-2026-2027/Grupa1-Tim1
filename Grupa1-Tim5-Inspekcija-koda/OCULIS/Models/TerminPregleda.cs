using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCULIS.Models
{
    public class TerminPregleda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Datum")]
        [DataType(DataType.Date)]
        public DateTime Datum { get; set; }

        [Required]
        [Display(Name = "Vrijeme")]
        public TimeSpan Vrijeme { get; set; }

        [Display(Name = "Status")]
        public StatusTermina Status { get; set; }

        [ForeignKey(nameof(Poslovnica))]
        public int IdPoslovnica { get; set; }
        public Poslovnica Poslovnica { get; set; } = null!;

        [ForeignKey(nameof(Korisnik))]
        public string IdKorisnik { get; set; } = string.Empty;
        public Korisnik Korisnik { get; set; } = null!;
    }
}
