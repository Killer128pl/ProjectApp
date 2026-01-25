using Microsoft.EntityFrameworkCore;
using ProjectApp.DataModel;

namespace ProjectApp.DataAccess.Database
{
    public class DatabaseDbContext : DbContext
    {
        public DatabaseDbContext(DbContextOptions<DatabaseDbContext> options) : base(options) { }

        public DbSet<Package> Packages { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Worker> Workers { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Package>(eb =>
            {
                eb.HasKey(p => p.TrackingNumber);
                eb.Property(p => p.TrackingNumber).ValueGeneratedNever(); // GUID podajemy sami
            });

            modelBuilder.Entity<Client>(eb =>
            {
                eb.HasKey(c => c.ClientId);
            });

            modelBuilder.Entity<Worker>(eb =>
            {
                eb.HasKey(w => w.WorkerId);
            });

            modelBuilder.Entity<Vehicle>(eb =>
            {
                eb.HasKey(v => v.VehicleId);
            });
        }
    }
}