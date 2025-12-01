using ProjectApp.Abstractions;
using ProjectApp.DataModel;

namespace ProjectApp.DataAccess.Memory.Repositories
{
    public class PackageRepositoryMemory : IPackageRepository
    {
        private readonly MemoryDbContext _db;

        public PackageRepositoryMemory(MemoryDbContext db) => _db = db;

        public IQueryable<Package> Query() => _db.Packages.AsQueryable();

        public Package? Get(Guid id) => _db.Packages.FirstOrDefault(p => p.TrackingNumber == id);

        public void Add(Package entity) => _db.Packages.Add(entity);

        public void Remove(Package entity) => _db.Packages.Remove(entity);

        public IEnumerable<Package> GetAll() => _db.Packages;
    }
}