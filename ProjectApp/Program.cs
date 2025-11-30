using System.Globalization;
using ProjectApp.Abstractions;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataAccess.Memory.Repositories;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Services;

var db = new MemoryDbContext();

IPackageRepository packageRepo = new PackageRepositoryMemory(db);

IPackageService packageSvc = new PackageService(packageRepo);

DataSeeder seeder = new DataSeeder(packageSvc);
seeder.Seed();

Console.WriteLine("Dane zseedowane. Dostepne paczki:");
ShowPackages(packageSvc, true);

static void ShowPackages(IPackageService packageSvc, bool showPackages)
{
    var packages = packageSvc.GetAll();
    if (packages.Any())
    {
        Console.WriteLine("Brak paczek.");
        return;
    }
    Console.WriteLine("---- Paczki: ----");

    foreach (var p in packages)
    {
        Console.WriteLine(p.ToString());
    }
}