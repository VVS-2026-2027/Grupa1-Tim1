using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using BentoLab.Services;

namespace BentoLab.Models
{
    public class Narudzba
    {
        public static double IzracunajPopust(double iznos)
        {
            var calculator = new PopustCalculator();
            decimal procenat = calculator.IzracunajProcenat((decimal)iznos);
            decimal popust = Math.Round((decimal)iznos * (procenat / 100m), 2, MidpointRounding.AwayFromZero);

            return (double)popust;
        }

        public double PrimijeniPopust(double iznosPopusta)
        {
            if (iznosPopusta < 0)
                throw new ArgumentException("Iznos popusta ne može biti negativan.", nameof(iznosPopusta));

            decimal umanjenaCijena = Math.Max(0m, (decimal)UkupnaCijena - (decimal)iznosPopusta);
            UkupnaCijena = (double)Math.Round(umanjenaCijena, 2, MidpointRounding.AwayFromZero);

            return UkupnaCijena;
        }

        [Key]

        public int NarudzbaID { get; set; }

        public double UkupnaCijena { get; set; }

        public double KoeficijentSlozenosti { get; set; }

        public StatusNarudzbe Status { get; set; }

        public DateTime DatumNarudzbe { get; set; }

        public DateTime DatumPreuzimanja { get; set; }
        public NacinPreuzimanja NacinPreuzimanja { get; set; }

        public int KorisnikID { get; set; }

        [ForeignKey("KorisnikID")]
        public Korisnik Korisnik { get; set; }

        public List<StavkaNarudzbe> Stavke { get; set; }

        public Dostava Dostava { get; set; }

        public List<Obavjestenje> Obavjestenja { get; set; }

        public Narudzba()
        {
        }
        public bool MozeSeOtkazati()
        {
            return Status == StatusNarudzbe.KREIRANA;
        }
    }
}