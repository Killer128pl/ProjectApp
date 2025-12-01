using ProjectApp.Abstractions;
using ProjectApp.Console.UI;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataAccess.Memory.Repositories;
using ProjectApp.Services;

// 1. Inicjalizacja Bazy i Repozytoriów
var db = new MemoryDbContext();
IPackageRepository packageRepo = new PackageRepositoryMemory(db);

// 2. Inicjalizacja Serwisów
var packageSvc = new PackageService(packageRepo);
var logisticsSvc = new LogisticsService(db);

// 3. Seedowanie danych (opcjonalne, ale przydatne)
var seeder = new DataSeeder(packageSvc, db);
seeder.Seed();

// 4. Uruchomienie Menu Głównego
var mainMenu = new MainMenu(packageSvc, logisticsSvc, db);
mainMenu.Run();