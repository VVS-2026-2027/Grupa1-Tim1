using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OCULIS.Models
{
    public class StavkaNarudzbe
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Narudzba))]
        public int IdNarudzba { get; set; }
        public Narudzba Narudzba { get; set; } = null!;

        public int IdProizvod { get; set; }
        public Proizvod? Proizvod { get; set; }

        [StringLength(150)]
        public string NazivProizvoda { get; set; } = string.Empty;

        public int Kolicina { get; set; }
        public double Cijena { get; set; }
    }
}
