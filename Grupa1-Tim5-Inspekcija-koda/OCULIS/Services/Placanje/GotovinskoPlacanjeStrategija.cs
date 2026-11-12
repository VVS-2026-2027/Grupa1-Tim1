using OCULIS.Models;

namespace OCULIS.Services.Placanje
{
    public class GotovinskoPlacanjeStrategija : IPlacanjeStrategija
    {
        public NacinPlacanja Nacin => NacinPlacanja.Gotovina;

        public Task<RezultatPlacanja> ObradiPlacanjeAsync(Models.Placanje placanje, string? brojKartice = null)
        {
            return Task.FromResult(new RezultatPlacanja
            {
                Uspjesno = true,
                Poruka = "Gotovinsko plaćanje evidentirano. Čeka potvrdu zaposlenika pri isporuci.",
                ReferencaTransakcije = $"CASH-{DateTime.UtcNow:yyyyMMddHHmmss}",
                Status = StatusPlacanja.NaCekanju
            });
        }
    }
}
