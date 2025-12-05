using ProjectApp.Console.Helpers;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Console.UIDictionary
{
    public class WarehouseMenu : MenuBase
    {
        private readonly IPackageService _packageService;
        private readonly Worker _worker;

        public WarehouseMenu(IPackageService packageService, Worker worker)
        {
            _packageService = packageService;
            _worker = worker;
        }

        protected override string Title => $"MAGAZYN: {_worker.LastName}";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Oczekujące na wydanie (Nadane)", ShowPending),
            ['2'] = new("Magazyn Uszkodzeń", ShowDamaged),
            ['3'] = new("Zgłoś uszkodzenie", ReportDamage),
            ['0'] = new("Wyloguj", null),
        };

        private void ShowPending()
        {
            var packs = _packageService.GetAll().Where(p => p.PackageStatus == PackageStatus.Nadana);
            if (!packs.Any()) System.Console.WriteLine("Brak.");
            foreach (var p in packs) System.Console.WriteLine(p);
            ConsoleHelpers.Pause();
        }

        private void ShowDamaged()
        {
            var packs = _packageService.GetAll().Where(p => p.PackageStatus == PackageStatus.Uszkodzona);
            if (!packs.Any()) System.Console.WriteLine("Brak.");
            foreach (var p in packs) System.Console.WriteLine(p);
            ConsoleHelpers.Pause();
        }

        private void ReportDamage()
        {
            var packs = _packageService.GetAll().Where(p => p.PackageStatus == PackageStatus.Nadana).ToList();
            if (!packs.Any()) return;

            for (int i = 0; i < packs.Count; i++) System.Console.WriteLine($"{i + 1}) {packs[i]}");
            int idx = ConsoleHelpers.ReadIndex("Uszkodzona paczka: ", packs.Count);
            if (idx < 0) return;

            _packageService.UpdatePackageStatus(packs[idx].TrackingNumber, PackageStatus.Uszkodzona);
            System.Console.WriteLine("Zgłoszono uszkodzenie.");
            ConsoleHelpers.Pause();
        }
    }
}