using Microsoft.Extensions.DependencyInjection;
using ProjectApp.Abstractions;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataAccess.Memory.Repositories;
using ProjectApp.Desktop.ViewModels;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Services;
using System;
using System.Windows;

namespace ProjectApp.Desktop
{
    public partial class App : Application
    {
        // Kontener serwisów dostępny publicznie (opcjonalnie)
        public static IServiceProvider Services { get; private set; } = default!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            // --- REJESTRACJA ZALEŻNOŚCI (DI) ---

            // 1. Baza danych (Singleton = jedna instancja na całą aplikację)
            services.AddSingleton<MemoryDbContext>();

            // 2. Repozytoria
            services.AddSingleton<IPackageRepository, PackageRepositoryMemory>();

            // 3. Serwisy Logiki Biznesowej
            services.AddSingleton<IPackageService, PackageService>();
            services.AddSingleton<LogisticsService>(); // Warto dodać, mimo że w tym oknie nie używamy

            // 4. Seeder (Dane startowe)
            services.AddSingleton<DataSeeder>();

            // 5. Widoki i ViewModele
            services.AddSingleton<MainWindow>();
            services.AddSingleton<PackagesViewModel>();

            // Budujemy kontener
            Services = services.BuildServiceProvider();

            // --- URUCHOMIENIE ---

            // 1. Wypełnij bazę danymi testowymi
            var seeder = Services.GetRequiredService<DataSeeder>();
            seeder.Seed();

            // 2. Pobierz główne okno z kontenera (z wstrzykniętym ViewModel) i pokaż je
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }
}