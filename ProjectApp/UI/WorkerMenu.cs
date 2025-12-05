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

        protected override string Title => $"KURIER: {_worker.LastName}";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Mój Pojazd", ShowMyVehicle),
            ['2'] = new("Moje Zlecenia", ShowMyPackages),
            ['3'] = new("Zgłoś dostarczenie/problem", UpdateStatus),
            ['0'] = new("Wyloguj", null),
        };

        private void ShowMyVehicle()
        {
            var v = _db.Vehicles.FirstOrDefault(x => x.VehicleId == _worker.AssignedVehicleId);
            if (v == null) System.Console.WriteLine("Brak auta.");
            else System.Console.WriteLine($"{v.Brand} {v.Model} ({v.RegNumber})");
            ConsoleHelpers.Pause();
        }

        private void ShowMyPackages()
        {
            var packs = _packageService.GetPackagesByWorker(_worker.WorkerId);
            foreach (var p in packs) System.Console.WriteLine(p);
            ConsoleHelpers.Pause();
        }

        private void UpdateStatus()
        {
            var packs = _packageService.GetPackagesByWorker(_worker.WorkerId).ToList();
            if (!packs.Any()) { System.Console.WriteLine("Pusto."); ConsoleHelpers.Pause(); return; }

            for (int i = 0; i < packs.Count; i++) System.Console.WriteLine($"{i + 1}) {packs[i]}");
            int idx = ConsoleHelpers.ReadIndex("Paczka: ", packs.Count);
            if (idx < 0) return;

            System.Console.WriteLine("2-Dostarczona, 3-Uszkodzona, 4-Zwrot");
            int s = (int)ConsoleHelpers.ReadFloat("Status: ");
            if (s >= 2 && s <= 4) _packageService.UpdatePackageStatus(packs[idx].TrackingNumber, (PackageStatus)s);
            ConsoleHelpers.Pause();
        }
    }
}