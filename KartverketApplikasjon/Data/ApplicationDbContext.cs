using KartverketApplikasjon.Models;
using Microsoft.EntityFrameworkCore;

namespace KartverketApplikasjon.Data
{
    public class ApplicationDbContext : DbContext
    {
        // Constructor that accepts options for configuring the context, such as the connection string
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // DbSets represent tables in the database
        public DbSet<UserData> Users { get; set; }
        public DbSet<GeoChange> GeoChanges { get; set; }
        public DbSet<MapCorrections> MapCorrections { get; set; }

        // Configure the entity models and relationships
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
