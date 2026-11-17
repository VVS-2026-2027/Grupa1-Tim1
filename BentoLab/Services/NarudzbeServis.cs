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
                throw new ArgumentNullException(nameof(narudzba), "Narudžba ne postoji.");
            }

            if (narudzba.Status == StatusNarudzbe.OTKAZANA)
            {
                throw new InvalidOperationException("Narudžba je već otkazana.");
            }

            if (!narudzba.MozeSeOtkazati())
            {
                throw new InvalidOperationException(
                    $"Narudžbu u statusu {narudzba.Status} nije moguće otkazati. Dozvoljeno je otkazivanje samo u statusu KREIRANA.");
            }
        }
    }
}