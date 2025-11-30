using ProjectApp.Abstractions;
using ProjectApp.DataModel;
using System.Reflection.Metadata;

namespace ProjectApp.DataAccess.Memory.Repositories
{
    public class PackageRepositoryMemory : IPackageRepository
    {
        private readonly MemoryDbContext _db;
        public PackageRepositoryMemory(MemoryDbContext db) => _db = db;
        public IQueryable<Package> Query() => _db.packages.AsQueryable();
        public Package? Get(Guid id) => _db.packages.FirstOrDefault(p => p.TrackingNumber == id);
        public void Add(Package entity) => _db.packages.Add(entity);
        public void Remove(Package entity) => _db.packages.Remove(entity);
        public IEnumerable<Package> GetAll() => _db.packages.GetAll();
    }
}
