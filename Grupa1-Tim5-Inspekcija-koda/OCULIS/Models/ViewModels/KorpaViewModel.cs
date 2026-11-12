namespace OCULIS.Models.ViewModels
{
    public class KorpaViewModel
    {
        public int KorpaId { get; set; }
        public List<StavkaKorpeViewModel> Stavke { get; set; } = new();
        public double UkupnaCijena { get; set; }
        public PopustRezultatViewModel? Popust { get; set; }
    }

    public class StavkaKorpeViewModel
    {
        public int Id { get; set; }
        public int IdProizvod { get; set; }
        public string NazivProizvoda { get; set; } = string.Empty;
        public int Kolicina { get; set; }
        public double Cijena { get; set; }
        public double Ukupno => Cijena * Kolicina;
    }
}
