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
        public static IServiceProvider Services { get; private set; } = default!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            services.AddSingleton<MemoryDbContext>();

            services.AddSingleton<IPackageRepository, PackageRepositoryMemory>();

            services.AddSingleton<IPackageService, PackageService>();
            services.AddSingleton<LogisticsService>();

            services.AddSingleton<DataSeeder>();

            services.AddSingleton<MainWindow>();
            services.AddSingleton<PackagesViewModel>();

            Services = services.BuildServiceProvider();

            Services.GetRequiredService<DataSeeder>().Seed();

            Services.GetRequiredService<MainWindow>().Show();
        }
    }
}