using ProjectApp.Console.Helpers;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Services;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Console.UIDictionary
{
    public class WarehouseMenu : MenuBase
    {
        private readonly IPackageService _packageService;
        private readonly LogisticsService _logisticsService;
        private readonly MemoryDbContext _db;
        private readonly Worker _worker;

        public WarehouseMenu(IPackageService packageService, LogisticsService logisticsService, MemoryDbContext db, Worker worker)
        {
            _packageService = packageService;
            _logisticsService = logisticsService;
            _db = db;
            _worker = worker;
        }

        protected override string Title => $"Magazyn: {_worker.LastName}";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Oczekujące na wydanie (Nadane)", ShowPending),
            ['2'] = new("Magazyn Uszkodzeń", ShowDamaged),
            ['3'] = new("Zgłoś uszkodzenie", ReportDamage),
            ['4'] = new("Wydaj paczkę kurierowi", AssignPackageToCourier),
            ['0'] = new("Wyloguj", null),
        };

        private void ShowPending()
        {
            System.Console.Clear();
            var packs = _packageService.GetAll().Where(p => p.PackageStatus == PackageStatus.Nadana);
            if (!packs.Any())
            {
                System.Console.WriteLine("Brak.");
                ConsoleHelpers.Pause();
                return;
            }

            foreach (var p in packs)
            {
                var client = _db.Clients.FirstOrDefault(c => c.ClientId == p.SenderId);
                string sender = client != null ? $"{client.LastName} {client.FirstName}" : "Nieznany";

                ConsoleHelpers.WriteColoredPackage(p, $"| Od: {sender}");
            }
            ConsoleHelpers.Pause();
        }

        private void ShowDamaged()
        {
            System.Console.Clear();
            var packs = _packageService.GetAll().Where(p => p.PackageStatus == PackageStatus.Uszkodzona);
            if (!packs.Any()) { System.Console.WriteLine("Brak."); ConsoleHelpers.Pause(); return; }

            foreach (var p in packs)
            {
                var client = _db.Clients.FirstOrDefault(c => c.ClientId == p.SenderId);
                string? sender = client != null ? client.LastName : "";
                ConsoleHelpers.WriteDestroyedPackage(p, $"({sender})");
            }
            ConsoleHelpers.Pause();
        }

        private void ReportDamage()
        {
            System.Console.Clear();
            var packs = _packageService.GetAll().Where(p => p.PackageStatus == PackageStatus.Nadana).ToList();
            if (!packs.Any()) return;

            for (int i = 0; i < packs.Count; i++)
            {
                var client = _db.Clients.FirstOrDefault(c => c.ClientId == packs[i].SenderId);
                string? sender = client != null ? client.LastName : "";

                System.Console.Write($"{i + 1}) ");
                ConsoleHelpers.WriteColoredPackage(packs[i], $"[{sender}]");
            }

            int idx = ConsoleHelpers.ReadIndex("Uszkodzona paczka: ", packs.Count);
            if (idx < 0) return;

            _packageService.UpdatePackageStatus(packs[idx].TrackingNumber, PackageStatus.Uszkodzona);
            System.Console.WriteLine("Zgłoszono uszkodzenie.");
            ConsoleHelpers.Pause();
        }

        private void AssignPackageToCourier()
        {
            System.Console.Clear();
            System.Console.WriteLine("--- Wydanie paczki kurierowi ---");

            var packages = _packageService.GetAll().Where(p => p.PackageStatus == PackageStatus.Nadana).ToList();
            var couriers = _db.Workers.Where(w => w.Position == "Kurier").ToList();

            if (!packages.Any())
            {
                System.Console.WriteLine("Brak paczek do wydania (Nadanych).");
                ConsoleHelpers.Pause();
                return;
            }
            if (!couriers.Any())
            {
                System.Console.WriteLine("Brak kurierów w systemie.");
                ConsoleHelpers.Pause();
                return;
            }

            System.Console.WriteLine("Paczki:");
            System.Console.Clear();
            for (int i = 0; i < packages.Count; i++)
            {
                var client = _db.Clients.FirstOrDefault(c => c.ClientId == packages[i].SenderId);
                string? sender = client != null ? client.LastName : "---";

                System.Console.Write($"{i + 1}) ");
                ConsoleHelpers.WriteColoredPackage(packages[i], $"[Nadawca: {sender}]");
            }

            int pIdx = ConsoleHelpers.ReadIndex("Wybierz paczkę: ", packages.Count);
            if (pIdx < 0) return;

            System.Console.WriteLine("\nKurierzy:");
            for (int i = 0; i < couriers.Count; i++)
            {
                string auto = couriers[i].AssignedVehicleId.HasValue ? "[Auto]" : "[Pieszy]";
                System.Console.WriteLine($"{i + 1}) {couriers[i].LastName} {couriers[i].FirstName} {auto}");
            }
            int cIdx = ConsoleHelpers.ReadIndex("Wybierz kuriera: ", couriers.Count);
            if (cIdx < 0) return;

            bool success = _logisticsService.AssignPackageToCourier(packages[pIdx].TrackingNumber, couriers[cIdx].WorkerId);

            if (success) System.Console.WriteLine("Sukces! Paczka przekazana kurierowi.");
            else System.Console.WriteLine("Błąd! Kurier prawdopodobnie nie ma auta.");

            ConsoleHelpers.Pause();
        }
    }
}