using Microsoft.EntityFrameworkCore;
using OCULIS.Data;
using OCULIS.Models;

namespace OCULIS.Services.Termin
{
    public interface ITerminServis
    {
        Task<bool> JeTerminDostupanAsync(int poslovnicaId, DateTime datum, TimeSpan vrijeme, int? excludeId = null);
        Task<List<TimeSpan>> DohvatiDostupneTermineAsync(int poslovnicaId, DateTime datum);
    }

    public class TerminServis : ITerminServis
    {
        private static readonly TimeSpan PocetakRadnogVremena = new(8, 0, 0);
        private static readonly TimeSpan KrajRadnogVremena = new(18, 0, 0);
        private static readonly TimeSpan TrajanjeTermina = new(0, 30, 0);

        private readonly ApplicationDbContext _context;

        public TerminServis(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> JeTerminDostupanAsync(int poslovnicaId, DateTime datum, TimeSpan vrijeme, int? excludeId = null)
        {
            if (vrijeme < PocetakRadnogVremena || vrijeme >= KrajRadnogVremena)
                return false;

            var query = _context.TerminPregleda
                .Where(t => t.IdPoslovnica == poslovnicaId
                    && t.Datum.Date == datum.Date
                    && t.Vrijeme == vrijeme
                    && t.Status != StatusTermina.Otkazan);

            if (excludeId.HasValue)
                query = query.Where(t => t.Id != excludeId.Value);

            return !await query.AnyAsync();
        }

        public async Task<List<TimeSpan>> DohvatiDostupneTermineAsync(int poslovnicaId, DateTime datum)
        {
            var zauzeti = await _context.TerminPregleda
                .Where(t => t.IdPoslovnica == poslovnicaId
                    && t.Datum.Date == datum.Date
                    && t.Status != StatusTermina.Otkazan)
                .Select(t => t.Vrijeme)
                .ToListAsync();

            var dostupni = new List<TimeSpan>();
            for (var slot = PocetakRadnogVremena; slot < KrajRadnogVremena; slot = slot.Add(TrajanjeTermina))
            {
                if (!zauzeti.Contains(slot))
                    dostupni.Add(slot);
            }

            return dostupni;
        }
    }
}
