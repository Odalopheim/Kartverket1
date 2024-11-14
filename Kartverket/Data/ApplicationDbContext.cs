using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Kartverket.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<PostSted> PostSteder { get; set; }
        public DbSet<Bruker> Brukere { get; set; }
        public DbSet<BrukerID> BrukerId { get; set; }
        public DbSet<Ansatt> Ansatte { get; set; }
        public DbSet<Innmelder> Innmeldere { get; set; }
        public DbSet<Innmelding> Innmeldinger { get; set; }
        public DbSet<Kategori> Kategorier { get; set; }
        public DbSet<GeoData> Geodata { get; set; }
        public DbSet<Gjest> Gjester { get; set; }
        public DbSet<GeoChange> GeoChange { get; set; }

         protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PostSted>()
                .HasMany(p => p.Brukere)
                .WithOne(b => b.PostSted)
                .HasForeignKey(b => b.PostNr);

            modelBuilder.Entity<Bruker>()
                .HasMany(b => b.Innmeldinger)
                .WithOne(i => i.Bruker)
                .HasForeignKey(i => i.BrukerId);

            modelBuilder.Entity<Kategori>()
                .HasMany(k => k.Innmeldinger)
                .WithOne(i => i.Kategori)
                .HasForeignKey(i => i.KatNr);
        }
    }
}
