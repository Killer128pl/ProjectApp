using ProjectApp.Console.Helpers;
using ProjectApp.Console.UIDictionary;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Console.UI
{
    public class AdminMenu : MenuBase
    {
        private readonly PackageMenu _packageMenu;
        private readonly LogisticsMenu _logisticsMenu;
        private readonly MemoryDbContext _db;
        private readonly IPackageService _packageSvc;

        public AdminMenu(IPackageService packageSvc, LogisticsService logisticsSvc, MemoryDbContext db)
        {
            _packageMenu = new PackageMenu(packageSvc, db);
            _logisticsMenu = new LogisticsMenu(logisticsSvc, db);
            _db = db;
            _packageSvc = packageSvc;
        }

        protected override string Title => "Panel zarządzania firmą (Admin)";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Zarządzanie magazynem (Paczki)", () => _packageMenu.Run()),
            ['2'] = new("Logistyka (Flota i Kurierzy)", () => _logisticsMenu.Run()),
            ['3'] = new("Zarządzanie Kadrami (Dodaj pracownika)", AddWorker),
            ['4'] = new("Zarządzanie Klientami (Dodaj klienta)", AddClient),
            ['5'] = new("Raport Dobowy (Cały system)", ShowGlobalReport),
            ['0'] = new("Wyloguj", null),
        };

        private void AddWorker()
        {
            System.Console.WriteLine("--- Nowy pracownik ---");
            string imie = ConsoleHelpers.ReadString("Imię: ");
            string nazwisko = ConsoleHelpers.ReadString("Nazwisko: ");

            System.Console.WriteLine("Stanowisko: 1 - Kurier, 2 - Magazynier");
            int role = (int)ConsoleHelpers.ReadFloat("Wybierz: ");
            string position = role == 1 ? "Kurier" : "Magazynier";

            _db.Workers.Add(new Worker { FirstName = imie, LastName = nazwisko, Position = position });
            System.Console.WriteLine($"Dodano pracownika: {imie} {nazwisko} ({position})");
            ConsoleHelpers.Pause();
        }

        private void AddClient()
        {
            System.Console.WriteLine("--- Nowy klient ---");
            string imie = ConsoleHelpers.ReadString("Imię: ");
            string nazwisko = ConsoleHelpers.ReadString("Nazwisko: ");
            _db.Clients.Add(new Client { FirstName = imie, LastName = nazwisko });
            System.Console.WriteLine("Dodano klienta.");
            ConsoleHelpers.Pause();
        }

        private void ShowGlobalReport()
        {
            var packages = _packageSvc.GetAll();
            System.Console.WriteLine($"=== Raport systemowy ===");
            System.Console.WriteLine($"Łączna liczba paczek: {packages.Count}");
            System.Console.WriteLine($"Nadane paczki: {packages.Count(p => p.PackageStatus == PackageStatus.Nadana)}");
            System.Console.WriteLine($"W trasie: {packages.Count(p => p.PackageStatus == PackageStatus.WTrasie)}");
            System.Console.WriteLine($"Dostarczone: {packages.Count(p => p.PackageStatus == PackageStatus.Dostarczona)}");
            System.Console.WriteLine($"Uszkodzone: {packages.Count(p => p.PackageStatus == PackageStatus.Uszkodzona)}");

            System.Console.WriteLine($"\nLiczba klientów: {_db.Clients.Count}");
            System.Console.WriteLine($"Liczba pracowników: {_db.Workers.Count}");
            ConsoleHelpers.Pause();
        }
    }
}