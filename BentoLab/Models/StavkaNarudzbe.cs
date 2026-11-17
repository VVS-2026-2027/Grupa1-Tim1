using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BentoLab.Models
{
    public class StavkaNarudzbe
    {
        [Key]
        public int StavkaID { get; set; }

        public int Kolicina { get; set; }

        public double CijenaStavke { get; set; }

        //VEZA NA NARUDZBU
        public int NarudzbaID { get; set; }

        [ForeignKey("NarudzbaID")]
        public Narudzba Narudzba { get; set; }

        //VEZA NA TORTU
        public int TortaID { get; set; }

        [ForeignKey("TortaID")]
        public Torta Torta { get; set; }

        public StavkaNarudzbe()
        {
        }
    }
}