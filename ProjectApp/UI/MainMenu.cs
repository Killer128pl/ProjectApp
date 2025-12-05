using ProjectApp.DataAccess.Memory;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Services;
using System.Collections.Generic;
using System;

namespace ProjectApp.Console.UI
{
    public class MainMenu : MenuBase
    {
        private readonly PackageMenu _packageMenu;
        private readonly LogisticsMenu _logisticsMenu;

        public MainMenu(IPackageService packageSvc, LogisticsService logisticsSvc, MemoryDbContext db)
        {
            _packageMenu = new PackageMenu(packageSvc, db);
            _logisticsMenu = new LogisticsMenu(logisticsSvc, db);
        }

        protected override string Title => "PANEL ADMINISTRATORA (ZARZĄDZANIE)";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Obsługa Paczek", () => _packageMenu.Run()),
            ['2'] = new("Logistyka (Flota i Kurierzy)", () => _logisticsMenu.Run()),
            ['0'] = new("Powrót do ekranu logowania", null),
        };
    }
}