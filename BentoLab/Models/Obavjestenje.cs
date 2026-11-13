using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BentoLab.Models
{
    public class Obavjestenje
    {
        [Key]
        public int ObavjestenjeID { get; set; }

        public string Sadrzaj { get; set; }

        public DateTime DatumSlanja { get; set; }

        //VEZA NA KORISNIKA
        public int KorisnikID { get; set; }

        [ForeignKey("KorisnikID")]
        public Korisnik Korisnik { get; set; }

        //VEZA NA NARUDZBU
        public int NarudzbaID { get; set; }

        [ForeignKey("NarudzbaID")]
        public Narudzba Narudzba { get; set; }

        public Obavjestenje()
        {
        }
    }
}