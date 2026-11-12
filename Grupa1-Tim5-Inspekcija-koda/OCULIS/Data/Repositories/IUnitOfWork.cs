using OCULIS.Models;

namespace OCULIS.Data.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Proizvod> Proizvodi { get; }
        IRepository<Korpa> Korpe { get; }
        IRepository<StavkaKorpe> StavkeKorpe { get; }
        IRepository<Narudzba> Narudzbe { get; }
        IRepository<Placanje> Placanja { get; }
        IRepository<Poslovnica> Poslovnice { get; }
        IRepository<TerminPregleda> Termini { get; }
        IRepository<ElektronskiKarton> Kartoni { get; }
        IRepository<PregledVida> Pregledi { get; }
        IRepository<Obavijest> Obavijesti { get; }
        IRepository<Reklamacija> Reklamacije { get; }
        IRepository<Akcija> Akcije { get; }
        ApplicationDbContext Context { get; }
        Task<int> SaveChangesAsync();
    }
}
