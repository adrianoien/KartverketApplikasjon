// Handles connection to the databse

using KartverketApplikasjon.Models;
using Microsoft.EntityFrameworkCore;

namespace KartverketApplikasjon.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<UserData> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserData>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}