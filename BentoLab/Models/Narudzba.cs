using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;


namespace BentoLab.Models
{
    public class Narudzba
    {
        public double PrimijeniPopust(double iznosPopusta)
        {
            if (iznosPopusta < 0)
            {
                throw new ArgumentException("Iznos popusta ne može biti negativan.", nameof(iznosPopusta));
            }

            decimal novaCijena = (decimal)UkupnaCijena - (decimal)iznosPopusta;
            if (novaCijena < 0m)
            {
                novaCijena = 0m;
            }

            UkupnaCijena = (double)Math.Round(novaCijena, 2, MidpointRounding.AwayFromZero);
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