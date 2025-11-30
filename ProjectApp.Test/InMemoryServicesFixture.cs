using ProjectApp.DataAccess.Memory;
using ProjectApp.DataAccess.Memory.Repositories;
using ProjectApp.Abstractions;
using ProjectApp.Services;
using ProjectApp.ServiceAbstractions;

public class InMemoryServicesFixture
{
    private DataSeeder _dataSeeder;
    public IPackageService PackageService { get; }

    public InMemoryServicesFixture()
    {
        var db = new MemoryDbContext();

        IPackageRepository packageRepo = new PackageRepositoryMemory(db);

        _dataSeeder = new DataSeeder(PackageService);
        _dataSeeder.Seed();
    }
}
