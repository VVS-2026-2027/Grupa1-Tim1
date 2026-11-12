using OCULIS.Models;

namespace OCULIS.Services.Popust
{
    public interface IPopustStrategija
    {
        string Naziv { get; }
        double IzracunajPopustPostotak(Korisnik korisnik, IEnumerable<StavkaKorpe> stavke, IEnumerable<Akcija> akcije);
    }
}
