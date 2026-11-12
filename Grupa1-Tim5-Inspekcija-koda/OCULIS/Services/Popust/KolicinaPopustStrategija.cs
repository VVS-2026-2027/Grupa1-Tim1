using OCULIS.Models;

namespace OCULIS.Services.Popust
{
    public class KolicinaPopustStrategija : IPopustStrategija
    {
        public string Naziv => "Količinski popust";

        public double IzracunajPopustPostotak(Korisnik korisnik, IEnumerable<StavkaKorpe> stavke, IEnumerable<Akcija> akcije)
        {
            var ukupnaKolicina = stavke.Sum(s => s.Kolicina);
            if (ukupnaKolicina >= 5) return 12;
            if (ukupnaKolicina >= 3) return 7;
            return 0;
        }
    }
}
