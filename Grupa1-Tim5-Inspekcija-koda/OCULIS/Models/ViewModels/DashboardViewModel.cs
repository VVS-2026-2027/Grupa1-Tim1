namespace OCULIS.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int UkupnoProizvoda { get; set; }
        public int UkupnoNarudzbi { get; set; }
        public int UkupnoTermina { get; set; }
        public int UkupnoKorisnika { get; set; }
        public double UkupniPrihod { get; set; }
        public List<NarudzbaStatistika> NedavneNarudzbe { get; set; } = new();
        public Dictionary<string, int> NarudzbePoStatusu { get; set; } = new();
    }

    public class NarudzbaStatistika
    {
        public int Id { get; set; }
        public string Korisnik { get; set; } = string.Empty;
        public double Iznos { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime Datum { get; set; }
    }
}
