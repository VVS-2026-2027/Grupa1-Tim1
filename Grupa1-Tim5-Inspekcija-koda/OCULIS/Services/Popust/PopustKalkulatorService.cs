using Microsoft.EntityFrameworkCore;
using OCULIS.Data;
using OCULIS.Models;
using OCULIS.Models.ViewModels;

namespace OCULIS.Services.Popust
{
    public interface IPopustKalkulatorService
    {
        Task<PopustRezultatViewModel> IzracunajPopustAsync(string korisnikId, int korpaId);
    }

    public class PopustKalkulatorService : IPopustKalkulatorService
    {
        private readonly ApplicationDbContext _context;
        private readonly IEnumerable<IPopustStrategija> _strategije;

        public PopustKalkulatorService(ApplicationDbContext context, IEnumerable<IPopustStrategija> strategije)
        {
            _context = context;
            _strategije = strategije;
        }

        public async Task<PopustRezultatViewModel> IzracunajPopustAsync(string korisnikId, int korpaId)
        {
            var korisnik = await _context.Users.FindAsync(korisnikId)
                ?? throw new InvalidOperationException("Korisnik nije pronađen.");

            var stavke = await _context.StavkaKorpe
                .Include(s => s.Proizvod)
                .Where(s => s.IdKorpa == korpaId)
                .ToListAsync();

            var akcije = await _context.Akcija.ToListAsync();
            var osnovnaCijena = stavke.Sum(s => s.Cijena * s.Kolicina);

            var primijenjeniPopusti = new List<string>();
            var maxPopust = 0.0;

            foreach (var strategija in _strategije)
            {
                var postotak = strategija.IzracunajPopustPostotak(korisnik, stavke, akcije);
                if (postotak > 0)
                {
                    primijenjeniPopusti.Add($"{strategija.Naziv}: {postotak}%");
                    maxPopust = Math.Max(maxPopust, postotak);
                }
            }

            var popustIznos = Math.Round(osnovnaCijena * maxPopust / 100, 2);
            var ukupnaCijena = Math.Round(osnovnaCijena - popustIznos, 2);

            return new PopustRezultatViewModel
            {
                OsnovnaCijena = osnovnaCijena,
                PopustPostotak = maxPopust,
                PopustIznos = popustIznos,
                UkupnaCijena = ukupnaCijena,
                PrimijenjeniPopusti = primijenjeniPopusti
            };
        }
    }
}
