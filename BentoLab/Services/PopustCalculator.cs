
using System;

namespace BentoLab.Services
{
    public class PopustCalculator
    {
        public decimal IzracunajProcenat(decimal iznos)
        {
            if (iznos < 0)
                throw new ArgumentOutOfRangeException(nameof(iznos));

            if (iznos >= 100)
                return 10m;

            if (iznos >= 50)
                return 5m;

            return 0m;
        }
    }
}
