using OCULIS.Models;

namespace OCULIS.Models.ViewModels
{
    public class ProizvodPretragaViewModel
    {
        public string? Pretraga { get; set; }
        public KategorijaProizvoda? Kategorija { get; set; }
        public string? Proizvodjac { get; set; }
        public double? MinCijena { get; set; }
        public double? MaxCijena { get; set; }
        public string? Sortiranje { get; set; }
        public List<Proizvod> Proizvodi { get; set; } = new();
    }
}
