using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;

namespace ProjectApp.Services
{
    public class DataSeeder : IDataSeeder
    {
        private readonly IPackageService _packageService;
        private readonly MemoryDbContext _db;

        public DataSeeder(IPackageService packageService, MemoryDbContext db)
        {
            _packageService = packageService;
            _db = db;
        }

        public SeedResult Seed()
        {
            var pckg1 = _packageService.CreatePackage(Guid.NewGuid(), DateTime.Now, 2.5f, "Big");
            var pckg2 = _packageService.CreatePackage(Guid.Parse("21372137-2137-2137-2137-213721372137"), DateTime.Now.AddDays(-1), 1.0f, "Small");
            var pckg3 = _packageService.CreatePackage(Guid.NewGuid(), DateTime.Now.AddDays(-2), 5.0f, "Huge");

            _db.Vehicles.Add(new Vehicle { Brand = "Ford", Model = "Transit", RegNumber = "WA 12345", VehicleStatus = "Available" });
            _db.Workers.Add(new Worker { FirstName = "Jan", LastName = "Kowalski", Position = "Kurier" });
            _db.Workers.Add(new Worker { FirstName = "Adam", LastName = "Nowak", Position = "Magazynier" });

            return new SeedResult { pckg1 = pckg1, pckg2 = pckg2, pckg3 = pckg3 };
        }
    }
}