using ProjectApp.Abstractions;
using ProjectApp.Console.UI;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataAccess.Memory.Repositories;
using ProjectApp.Services;


var db = new MemoryDbContext();
IPackageRepository packageRepo = new PackageRepositoryMemory(db);


var packageSvc = new PackageService(packageRepo);
var logisticsSvc = new LogisticsService(db);


var seeder = new DataSeeder(packageSvc, db);
seeder.Seed();


var startMenu = new StartMenu(packageSvc, logisticsSvc, db);
startMenu.Run();