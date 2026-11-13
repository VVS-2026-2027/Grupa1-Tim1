using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BentoLab.Models
{
    public class Dostava
    {
        [Key]
        public int DostavaID { get; set; }

        public string Adresa { get; set; }

        public string KontaktTelefon { get; set; }

        public double CijenaDostave { get; set; }

        public DateTime VrijemeIsporuke { get; set; }

        public string Napomena { get; set; }

        public int NarudzbaID { get; set; }

        [ForeignKey("NarudzbaID")]
        public Narudzba Narudzba { get; set; }

        public Dostava()
        {
        }
    }
}