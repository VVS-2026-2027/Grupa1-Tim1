using System.ComponentModel.DataAnnotations;

namespace BentoLab.Models
{
    public class Torta
    {
        [Key]
        public int TortaID { get; set; }

        public string Naziv { get; set; }

        public double Cijena { get; set; }

        public int KolicinaNaStanju { get; set; }

        public bool Dostupna { get; set; }
    }
}