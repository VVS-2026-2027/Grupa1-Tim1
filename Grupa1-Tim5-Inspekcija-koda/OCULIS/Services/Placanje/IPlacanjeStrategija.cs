using OCULIS.Models;

namespace OCULIS.Services.Placanje
{
    public interface IPlacanjeStrategija
    {
        NacinPlacanja Nacin { get; }
        Task<RezultatPlacanja> ObradiPlacanjeAsync(Models.Placanje placanje, string? brojKartice = null);
    }

    public class RezultatPlacanja
    {
        public bool Uspjesno { get; set; }
        public string Poruka { get; set; } = string.Empty;
        public string? ReferencaTransakcije { get; set; }
        public StatusPlacanja Status { get; set; }
    }
}
