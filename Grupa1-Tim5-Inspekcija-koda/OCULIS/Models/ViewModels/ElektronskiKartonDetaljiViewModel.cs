using OCULIS.Models;

namespace OCULIS.Models.ViewModels
{
    public class ElektronskiKartonDetaljiViewModel
    {
        public ElektronskiKarton Karton { get; set; } = null!;
        public List<PregledVida> Pregledi { get; set; } = new();
        public List<Narudzba> Narudzbe { get; set; } = new();
        public List<Reklamacija> Reklamacije { get; set; } = new();
    }
}
