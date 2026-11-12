namespace OCULIS.Models.ViewModels
{
    public class PopustRezultatViewModel
    {
        public double OsnovnaCijena { get; set; }
        public double PopustPostotak { get; set; }
        public double PopustIznos { get; set; }
        public double UkupnaCijena { get; set; }
        public List<string> PrimijenjeniPopusti { get; set; } = new();
    }
}
