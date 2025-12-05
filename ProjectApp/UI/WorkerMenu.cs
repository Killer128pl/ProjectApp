using ProjectApp.Console.Helpers;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;

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

        protected override string Title => $"PANEL PRACOWNIKA: {_worker.Position.ToUpper()} {_worker.LastName}";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Mój Pojazd Służbowy", ShowMyVehicle),
            ['2'] = new("Moje Zlecenia (Paczki)", ShowMyPackages),
            ['3'] = new("Raportuj Status Paczki", UpdateStatus),
            ['0'] = new("Koniec Zmiany (Wyloguj)", null),
        };

        private void ShowMyVehicle()
        {
            if (_worker.AssignedVehicleId == null)
            {
                System.Console.WriteLine("Nie masz przypisanego pojazdu. Udaj się do Dyspozytora (Admina).");
            }
            else
            {
                var v = _db.Vehicles.FirstOrDefault(x => x.VehicleId == _worker.AssignedVehicleId);
                System.Console.WriteLine("--- PRZYPISANY POJAZD ---");
                System.Console.WriteLine($"Marka: {v?.Brand} {v?.Model}");
                System.Console.WriteLine($"Rejestracja: {v?.RegNumber}");
                System.Console.WriteLine($"Status: {v?.VehicleStatus}");
            }
            ConsoleHelpers.Pause();
        }

        private void ShowMyPackages()
        {
            var packs = _packageService.GetPackagesByWorker(_worker.WorkerId);
            if (!packs.Any())
            {
                System.Console.WriteLine("Brak paczek na pace. Możesz odpocząć.");
                ConsoleHelpers.Pause();
                return;
            }

            System.Console.WriteLine("--- LISTA PRZESYŁEK NA AUCIE ---");
            foreach (var p in packs)
            {
                System.Console.WriteLine(p);
            }
            ConsoleHelpers.Pause();
        }

        private void UpdateStatus()
        {
            var packs = _packageService.GetPackagesByWorker(_worker.WorkerId).ToList();
            if (!packs.Any())
            {
                System.Console.WriteLine("Nie masz paczek, więc nie możesz zmienić statusu.");
                ConsoleHelpers.Pause();
                return;
            }

            for (int i = 0; i < packs.Count; i++)
                System.Console.WriteLine($"{i + 1}) {packs[i].TrackingNumber} [{packs[i].PackageStatus}]");

            int idx = ConsoleHelpers.ReadIndex("Wybierz paczkę: ", packs.Count);
            if (idx < 0) return;

            System.Console.WriteLine("\nNowy status:");
            System.Console.WriteLine("2 - Dostarczona (Delivered)");
            System.Console.WriteLine("3 - Uszkodzona (Damaged)");

            float st = ConsoleHelpers.ReadFloat("Wybierz opcję: ");

            if (st == 2 || st == 3)
            {
                _packageService.UpdatePackageStatus(packs[idx].TrackingNumber, (PackageStatus)st);
                System.Console.WriteLine("Status zaktualizowany w systemie.");
            }
            else
            {
                System.Console.WriteLine("Anulowano lub niepoprawny status.");
            }
            ConsoleHelpers.Pause();
        }
    }
}