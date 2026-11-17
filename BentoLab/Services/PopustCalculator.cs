
using System;

namespace BentoLab.Services
{
    public class PopustCalculator
    {
        private const decimal GranicaZaMaliPopust = 50m;
        private const decimal GranicaZaVelikiPopust = 100m;

        private const decimal MaliPopust = 5m;
        private const decimal VelikiPopust = 10m;

        public decimal IzracunajProcenat(decimal iznos)
        {
            if (iznos < 0)
                throw new ArgumentOutOfRangeException(nameof(iznos));

            if (iznos >= GranicaZaVelikiPopust)
                return VelikiPopust;

            if (iznos >= GranicaZaMaliPopust)
                return MaliPopust;

            return 0m;
        }
    }
}
