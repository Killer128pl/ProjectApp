using ProjectApp.Abstractions;
using ProjectApp.Console.Helpers;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ProjectApp.Console.UIDictionary
{
    public class PackageMenu : MenuBase
    {
        private readonly IPackageService _packageService;
        private readonly MemoryDbContext _db;

        public PackageMenu(IPackageService packageService, MemoryDbContext db)
        {
            _packageService = packageService;
            _db = db;
        }

        protected override string Title => "Panel zarządzania firmą (Admin)";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Lista wszystkich paczek", ShowAllPackages),
            ['2'] = new("Nadaj paczkę (dla Klienta)", CreatePackage),
            ['3'] = new("Zmień status (Logistyka)", UpdateStatus),
            ['4'] = new("Zmień status płatności", UpdatePaymentStatus),
            ['0'] = new("Powrót", null),
        };

        private void ShowAllPackages()
        {
            var p = _packageService.GetAll();
            if (!p.Any()) System.Console.WriteLine("Brak paczek.");
            foreach (var item in p) System.Console.WriteLine(item);
            ConsoleHelpers.Pause();
        }

        private void CreatePackage()
        {
            var clients = _db.Clients.ToList();
            if (!clients.Any()) { System.Console.WriteLine("Brak klientów."); ConsoleHelpers.Pause(); return; }

            for (int i = 0; i < clients.Count; i++)
                System.Console.WriteLine($"{i + 1}) {clients[i].FirstName} {clients[i].LastName}");

            int cIdx = ConsoleHelpers.ReadIndex("Wybierz nadawcę: ", clients.Count);
            if (cIdx < 0) return;

            float w = ConsoleHelpers.ReadFloat("Waga (kg): ");
            string s = ConsoleHelpers.ReadString("Rozmiar: ");
            var id = Guid.NewGuid();
            _packageService.CreatePackage(id, clients[cIdx].ClientId, DateTime.Now, w, s);
            System.Console.WriteLine("Utworzono.");
            ConsoleHelpers.Pause();
        }

        private void UpdateStatus()
        {
            var packs = _packageService.GetAll().ToList();
            if (!packs.Any()) return;

            for (int i = 0; i < packs.Count; i++) System.Console.WriteLine($"{i + 1}) {packs[i]}");
            int idx = ConsoleHelpers.ReadIndex("Wybierz: ", packs.Count);
            if (idx < 0) return;

            System.Console.WriteLine("0 - Nadana, 1 - W Trasie, 2 - Dostarczona, 3 - Uszkodzona");
            int s = (int)ConsoleHelpers.ReadFloat("Status: ");
            if (s >= 0 && s <= 4) _packageService.UpdatePackageStatus(packs[idx].TrackingNumber, (PackageStatus)s);
            ConsoleHelpers.Pause();
        }

        private void UpdatePaymentStatus()
        {
            var packs = _packageService.GetAll().ToList();
            for (int i = 0; i < packs.Count; i++) System.Console.WriteLine($"{i + 1}) {packs[i]}");
            int idx = ConsoleHelpers.ReadIndex("Wybierz: ", packs.Count);
            if (idx < 0) return;

            System.Console.WriteLine("0 - Oplacona, 1 - Nieoplacona, 2 - Płatność Przy Odbiorze");
            int s = (int)ConsoleHelpers.ReadFloat("Status: ");
            if (s >= 0 && s <= 2) _packageService.UpdatePaymentStatus(packs[idx].TrackingNumber, (PaymentStatus)s);
            ConsoleHelpers.Pause();
        }
    }
}