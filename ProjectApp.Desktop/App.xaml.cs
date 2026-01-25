using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectApp.Abstractions;
using ProjectApp.DataAccess.Database;
using ProjectApp.DataAccess.Database.Repositories;
using ProjectApp.Desktop.ViewModels;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Services;
using System;
using System.Windows;

namespace ProjectApp.Desktop
{
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = default!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            var connectionString = "Server=(localdb)\\mssqllocaldb;Database=ProjectAppDb;Trusted_Connection=True;MultipleActiveResultSets=true";

            services.AddDbContext<DatabaseDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped<IUnitOfWork, UnitOfWorkDatabase>();

            services.AddScoped<IPackageRepository, PackageRepositoryDatabase>();

            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<LogisticsService>();
            services.AddScoped<DataSeeder>(provider => new DataSeeder(provider.GetRequiredService<IPackageService>(), null, provider.GetRequiredService<DatabaseDbContext>()));

            services.AddTransient<MainWindow>();
            services.AddTransient<PackagesViewModel>();

            Services = services.BuildServiceProvider();


            EnsureDatabaseCreated();

            SeedDataIfEmpty();

            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void EnsureDatabaseCreated()
        {
            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DatabaseDbContext>();
                db.Database.EnsureCreated();
            }
        }

        private void SeedDataIfEmpty()
        {
            using (var scope = Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<DatabaseDbContext>();
                if (!db.Packages.AnyAsync().Result)
                {
                    var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                    seeder.Seed();
                }
            }
        }
    }
}