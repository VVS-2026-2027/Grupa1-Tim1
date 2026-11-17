using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BentoLab.Models
{
    public class Korpa
    {
        [Key]
        public int KorpaID { get; set; }

        public DateTime DatumKreiranja { get; set; }

        public int KorisnikID { get; set; }

        [ForeignKey("KorisnikID")]
        public Korisnik Korisnik { get; set; }

        public Korpa()
        {
        }
    }
}