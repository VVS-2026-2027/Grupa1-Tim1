using System;
using BentoLab.Models;

namespace BentoLab.Services
{
    public class NarudzbaServis
    {
        public void ValidirajOtkazivanje(Narudzba narudzba)
        {
            if (narudzba == null)
            {
                throw new ArgumentNullException(nameof(narudzba));
            }

            if (narudzba.Status != StatusNarudzbe.KREIRANA)
            {
                throw new InvalidOperationException("Otkazivanje je dozvoljeno samo za narudžbu u statusu KREIRANA.");
            }
        }
    }
}