namespace OCULIS.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            Proizvodi = new Repository<Models.Proizvod>(_context);
            Korpe = new Repository<Models.Korpa>(_context);
            StavkeKorpe = new Repository<Models.StavkaKorpe>(_context);
            Narudzbe = new Repository<Models.Narudzba>(_context);
            Placanja = new Repository<Models.Placanje>(_context);
            Poslovnice = new Repository<Models.Poslovnica>(_context);
            Termini = new Repository<Models.TerminPregleda>(_context);
            Kartoni = new Repository<Models.ElektronskiKarton>(_context);
            Pregledi = new Repository<Models.PregledVida>(_context);
            Obavijesti = new Repository<Models.Obavijest>(_context);
            Reklamacije = new Repository<Models.Reklamacija>(_context);
            Akcije = new Repository<Models.Akcija>(_context);
        }

        public IRepository<Models.Proizvod> Proizvodi { get; }
        public IRepository<Models.Korpa> Korpe { get; }
        public IRepository<Models.StavkaKorpe> StavkeKorpe { get; }
        public IRepository<Models.Narudzba> Narudzbe { get; }
        public IRepository<Models.Placanje> Placanja { get; }
        public IRepository<Models.Poslovnica> Poslovnice { get; }
        public IRepository<Models.TerminPregleda> Termini { get; }
        public IRepository<Models.ElektronskiKarton> Kartoni { get; }
        public IRepository<Models.PregledVida> Pregledi { get; }
        public IRepository<Models.Obavijest> Obavijesti { get; }
        public IRepository<Models.Reklamacija> Reklamacije { get; }
        public IRepository<Models.Akcija> Akcije { get; }
        public ApplicationDbContext Context => _context;

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}
