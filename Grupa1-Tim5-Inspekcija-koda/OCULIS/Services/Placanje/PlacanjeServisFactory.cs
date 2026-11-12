using OCULIS.Models;

namespace OCULIS.Services.Placanje
{
    public interface IPlacanjeServisFactory
    {
        IPlacanjeStrategija Kreiraj(NacinPlacanja nacin);
    }

    public class PlacanjeServisFactory : IPlacanjeServisFactory
    {
        private readonly IEnumerable<IPlacanjeStrategija> _strategije;

        public PlacanjeServisFactory(IEnumerable<IPlacanjeStrategija> strategije)
        {
            _strategije = strategije;
        }

        public IPlacanjeStrategija Kreiraj(NacinPlacanja nacin)
        {
            return _strategije.First(s => s.Nacin == nacin);
        }
    }
}
