using OCULIS.Models;

namespace OCULIS.Services.Popust
{
    public class AkcijaPopustStrategija : IPopustStrategija
    {
        public string Naziv => "Akcijski popust";

        public double IzracunajPopustPostotak(Korisnik korisnik, IEnumerable<StavkaKorpe> stavke, IEnumerable<Akcija> akcije)
        {
            var sada = DateTime.Now;
            var aktivneAkcije = akcije
                .Where(a => a.Aktivna && a.DatumPocetka <= sada && a.DatumZavrsetka >= sada)
                .ToList();

            if (!aktivneAkcije.Any()) return 0;

            var proizvodIds = stavke.Select(s => s.IdProizvod).ToHashSet();
            var relevantne = aktivneAkcije
                .Where(a => a.IdProizvod == null || proizvodIds.Contains(a.IdProizvod.Value))
                .Select(a => a.PopustPostotak);

            return relevantne.DefaultIfEmpty(0).Max();
        }
    }
}
