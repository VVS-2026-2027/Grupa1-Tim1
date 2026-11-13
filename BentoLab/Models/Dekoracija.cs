using System.ComponentModel.DataAnnotations;

namespace BentoLab.Models
{
    public class Dekoracija
    {
        [Key]
        public int DekoracijaID { get; set; }

        public string Naziv { get; set; }

        public double Cijena { get; set; }

        public TipDekoracije TipDekoracije { get; set; }
    }
}