using ProjectApp.DataAccess.Memory;
using ProjectApp.DataAccess.Memory.Repositories;
using ProjectApp.Abstractions;
using ProjectApp.Services;
using ProjectApp.ServiceAbstractions;

namespace ProjectApp.Test
{
    public class InMemoryServicesFixture
    {
        public IPackageService PackageService { get; }
        public MemoryDbContext Db { get; }

        public InMemoryServicesFixture()
        {
            Db = new MemoryDbContext();

            IPackageRepository packageRepo = new PackageRepositoryMemory(Db);
            PackageService = new PackageService(packageRepo);

            var dataSeeder = new DataSeeder(PackageService, Db);
            dataSeeder.Seed();
        }
    }
}