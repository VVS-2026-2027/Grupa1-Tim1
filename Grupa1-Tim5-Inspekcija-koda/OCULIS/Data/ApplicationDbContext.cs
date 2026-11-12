using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OCULIS.Models;

namespace OCULIS.Data
{
    public class ApplicationDbContext : IdentityDbContext<Korisnik>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Proizvod> Proizvod { get; set; }
        public DbSet<Korpa> Korpa { get; set; }
        public DbSet<StavkaKorpe> StavkaKorpe { get; set; }
        public DbSet<Narudzba> Narudzba { get; set; }
        public DbSet<Placanje> Placanje { get; set; }
        public DbSet<Obavijest> Obavijest { get; set; }
        public DbSet<Poslovnica> Poslovnica { get; set; }
        public DbSet<TerminPregleda> TerminPregleda { get; set; }
        public DbSet<ElektronskiKarton> ElektronskiKarton { get; set; }
        public DbSet<PregledVida> PregledVida { get; set; }
        public DbSet<Reklamacija> Reklamacija { get; set; }
        public DbSet<Akcija> Akcija { get; set; }
        public DbSet<StavkaNarudzbe> StavkaNarudzbe { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Proizvod>().ToTable("Proizvod");
            modelBuilder.Entity<Korpa>().ToTable("Korpa");
            modelBuilder.Entity<StavkaKorpe>().ToTable("StavkaKorpe");
            modelBuilder.Entity<Narudzba>().ToTable("Narudzba");
            modelBuilder.Entity<Placanje>().ToTable("Placanje");
            modelBuilder.Entity<Obavijest>().ToTable("Obavijest");
            modelBuilder.Entity<Poslovnica>().ToTable("Poslovnica");
            modelBuilder.Entity<TerminPregleda>().ToTable("TerminPregleda");
            modelBuilder.Entity<ElektronskiKarton>().ToTable("ElektronskiKarton");
            modelBuilder.Entity<PregledVida>().ToTable("PregledVida");
            modelBuilder.Entity<Reklamacija>().ToTable("Reklamacija");
            modelBuilder.Entity<Akcija>().ToTable("Akcija");
            modelBuilder.Entity<StavkaNarudzbe>().ToTable("StavkaNarudzbe");

            modelBuilder.Entity<Narudzba>()
                .HasMany(n => n.Stavke)
                .WithOne(s => s.Narudzba)
                .HasForeignKey(s => s.IdNarudzba)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<StavkaNarudzbe>()
                .HasOne(s => s.Proizvod)
                .WithMany()
                .HasForeignKey(s => s.IdProizvod)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Korpa>()
                .HasOne(k => k.Korisnik)
                .WithMany()
                .HasForeignKey(k => k.IdKorisnik)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Korpa>()
                .HasMany(k => k.Stavke)
                .WithOne(s => s.Korpa)
                .HasForeignKey(s => s.IdKorpa)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Narudzba>()
                .HasOne(n => n.Korisnik)
                .WithMany()
                .HasForeignKey(n => n.IdKorisnik)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Narudzba>()
                .HasOne(n => n.Korpa)
                .WithMany()
                .HasForeignKey(n => n.IdKorpa)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Placanje>()
                .HasOne(p => p.Narudzba)
                .WithMany(n => n.Placanja)
                .HasForeignKey(p => p.IdNarudzba)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TerminPregleda>()
                .HasOne(t => t.Korisnik)
                .WithMany()
                .HasForeignKey(t => t.IdKorisnik)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TerminPregleda>()
                .HasOne(t => t.Poslovnica)
                .WithMany()
                .HasForeignKey(t => t.IdPoslovnica)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ElektronskiKarton>()
                .HasOne(e => e.Korisnik)
                .WithMany()
                .HasForeignKey(e => e.IdKorisnik)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ElektronskiKarton>()
                .HasMany(e => e.Pregledi)
                .WithOne(p => p.ElektronskiKarton)
                .HasForeignKey(p => p.IdElektronskiKarton)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Reklamacija>()
                .HasOne(r => r.Korisnik)
                .WithMany()
                .HasForeignKey(r => r.IdKorisnik)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Akcija>()
                .HasOne(a => a.Proizvod)
                .WithMany()
                .HasForeignKey(a => a.IdProizvod)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
