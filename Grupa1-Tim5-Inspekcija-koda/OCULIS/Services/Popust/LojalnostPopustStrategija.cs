using OCULIS.Models;

namespace OCULIS.Services.Popust
{
    public class LojalnostPopustStrategija : IPopustStrategija
    {
        public string Naziv => "Lojalnost korisnika";

        public double IzracunajPopustPostotak(Korisnik korisnik, IEnumerable<StavkaKorpe> stavke, IEnumerable<Akcija> akcije)
        {
            if (korisnik.BrojNarudzbi >= 10 || korisnik.LojalnostBodovi >= 500)
                return 15;
            if (korisnik.BrojNarudzbi >= 5 || korisnik.LojalnostBodovi >= 200)
                return 10;
            if (korisnik.BrojNarudzbi >= 2)
                return 5;
            return 0;
        }
    }
}
