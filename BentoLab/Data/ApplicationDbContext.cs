using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BentoLab.Models;
using Microsoft.AspNetCore.Identity;

namespace BentoLab.Data
{
    public class ApplicationDbContext : IdentityDbContext<Korisnik, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
        {
        }

        public DbSet<Narudzba> Narudzbe { get; set; }
        public DbSet<StavkaNarudzbe> StavkeNarudzbe { get; set; }
        public DbSet<Korpa> Korpe { get; set; }
        public DbSet<Dostava> Dostave { get; set; }
        public DbSet<Obavjestenje> Obavjestenja { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Obavjestenje>()
            .HasOne(o => o.Korisnik)
            .WithMany(k => k.Obavjestenja)
            .HasForeignKey(o => o.KorisnikID)
            .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Obavjestenje>()
            .HasOne(o => o.Narudzba)
            .WithMany(n => n.Obavjestenja)
            .HasForeignKey(o => o.NarudzbaID)
            .OnDelete(DeleteBehavior.NoAction);
        }
        public DbSet<BentoLab.Models.Korisnik> Korisnik { get; set; } = default!;
        public DbSet<BentoLab.Models.Torta> Torta { get; set; } = default!;
        public DbSet<BentoLab.Models.Dekoracija> Dekoracija { get; set; } = default!;
    }
}