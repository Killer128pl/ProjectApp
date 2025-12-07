using ProjectApp.Console.Helpers;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System.Linq;
using System.Collections.Generic;
using ProjectApp.Console.UIDictionary;

namespace ProjectApp.Console.UI
{
    public class WorkerMenu : MenuBase
    {
        private readonly IPackageService _packageService;
        private readonly MemoryDbContext _db;
        private readonly Worker _worker;

        public WorkerMenu(IPackageService packageService, MemoryDbContext db, Worker worker)
        {
            _packageService = packageService;
            _db = db;
            _worker = worker;
        }

        protected override string Title => $"Kurier: {_worker.LastName}";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Mój Pojazd", ShowMyVehicle),
            ['2'] = new("Moje Zlecenia (Aktywne)", ShowMyPackages),
            ['3'] = new("Zgłoś dostarczenie/problem", UpdateStatus),
            ['0'] = new("Wyloguj", null),
        };

        private void ShowMyVehicle()
        {
            System.Console.Clear();
            var v = _db.Vehicles.FirstOrDefault(x => x.VehicleId == _worker.AssignedVehicleId);
            if (v == null) System.Console.WriteLine("Brak auta.");
            else System.Console.WriteLine($"{v.Brand} {v.Model} ({v.RegNumber})");
            ConsoleHelpers.Pause();
        }

        private void ShowMyPackages()
        {
            System.Console.Clear();
            var packs = _packageService.GetPackagesByWorker(_worker.WorkerId)
                        .Where(p => p.PackageStatus != PackageStatus.Dostarczona && p.PackageStatus != PackageStatus.Uszkodzona)
                        .ToList();

            if (!packs.Any())
            {
                System.Console.WriteLine("Brak aktywnych zleceń. Wszystko rozwiezione!");
                ConsoleHelpers.Pause();
                return;
            }

            foreach (var p in packs)
            {
                var client = _db.Clients.FirstOrDefault(c => c.ClientId == p.SenderId);
                string sender = client != null ? $"{client.LastName} {client.FirstName}" : "Brak danych";

                ConsoleHelpers.WriteColoredPackage(p);
                System.Console.WriteLine($"   -> Odbiór od: {sender}");
            }
            ConsoleHelpers.Pause();
        }

        private void UpdateStatus()
        {
            System.Console.Clear();
            var packs = _packageService.GetPackagesByWorker(_worker.WorkerId)
                                       .Where(p => p.PackageStatus != PackageStatus.Dostarczona)
                                       .ToList();

            if (!packs.Any())
            {
                System.Console.WriteLine("Brak paczek do edycji (wszystkie dostarczone).");
                ConsoleHelpers.Pause();
                return;
            }

            for (int i = 0; i < packs.Count; i++)
            {
                var client = _db.Clients.FirstOrDefault(c => c.ClientId == packs[i].SenderId);
                string? sender = client != null ? client.LastName : "";

                System.Console.Write($"{i + 1}) ");
                ConsoleHelpers.WriteColoredPackage(packs[i], $"({sender})");
            }

            int idx = ConsoleHelpers.ReadIndex("Paczka: ", packs.Count);

            System.Console.WriteLine("2 - Dostarczona, 3 - Uszkodzona");
            int s = ConsoleHelpers.ReadInt("Wybierz nowy status: ", 2, 3);

            _packageService.UpdatePackageStatus(packs[idx].TrackingNumber, (PackageStatus)s);

            if (s == 2)
            {
                System.Console.WriteLine("Paczka oznaczona jako dostarczona.");
            }
            else
            {
                System.Console.WriteLine("Status zaktualizowany.");
            }
            ConsoleHelpers.Pause();
        }
    }
}