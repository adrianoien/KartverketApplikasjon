using Microsoft.EntityFrameworkCore;
using KartverketApplikasjon.Data; 
namespace KartverketApplikasjon.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<GeoChange> GeoChanges { get; set; }
    }
}