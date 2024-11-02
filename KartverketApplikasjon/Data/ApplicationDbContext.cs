using KartverketApplikasjon.Models;
using Microsoft.EntityFrameworkCore;

namespace KartverketApplikasjon.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }


        public DbSet<UserData> Users { get; set; }
        public DbSet<GeoChange> GeoChanges { get; set; }
        public DbSet<MapCorrections> MapCorrections { get; set; }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserData>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<MapCorrections>(entity =>
            {
                entity.HasKey(e => e.Id);
            });

            modelBuilder.Entity<GeoChange>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }
}
