using OCULIS.Models;

namespace OCULIS.Services.Placanje
{
    /// <summary>
    /// Simulacija eksternog servisa za autorizaciju kartičnog plaćanja.
    /// </summary>
    public class KarticnoPlacanjeStrategija : IPlacanjeStrategija
    {
        public NacinPlacanja Nacin => NacinPlacanja.KreditnaKartica;

        public async Task<RezultatPlacanja> ObradiPlacanjeAsync(Models.Placanje placanje, string? brojKartice = null)
        {
            await Task.Delay(1500);

            if (string.IsNullOrWhiteSpace(brojKartice) || brojKartice.Length < 13)
            {
                return new RezultatPlacanja
                {
                    Uspjesno = false,
                    Poruka = "Neispravan broj kartice. Autorizacija odbijena.",
                    Status = StatusPlacanja.Odbijeno
                };
            }

            var zadnjeCifre = brojKartice[^4..];
            if (zadnjeCifre == "0000")
            {
                return new RezultatPlacanja
                {
                    Uspjesno = false,
                    Poruka = "Transakcija odbijena od strane banke (nedovoljna sredstva).",
                    Status = StatusPlacanja.Odbijeno
                };
            }

            var referenca = $"TXN-{DateTime.UtcNow:yyyyMMddHHmmss}-{Random.Shared.Next(1000, 9999)}";
            return new RezultatPlacanja
            {
                Uspjesno = true,
                Poruka = $"Plaćanje karticom uspješno autorizovano. Kartica ****{zadnjeCifre}",
                ReferencaTransakcije = referenca,
                Status = StatusPlacanja.Uspjesno
            };
        }
    }
}
