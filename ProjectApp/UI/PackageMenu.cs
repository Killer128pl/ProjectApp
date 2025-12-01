using ProjectApp.Abstractions;
using ProjectApp.Console.Helpers;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;

namespace ProjectApp.Console.UI
{
    public class PackageMenu : MenuBase
    {
        private readonly IPackageService _packageService;

        public PackageMenu(IPackageService packageService)
        {
            _packageService = packageService;
        }

        protected override string Title => "ZARZĄDZANIE PACZKAMI";
        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Lista wszystkich paczek", ShowAllPackages),
            ['2'] = new("Nadaj nową paczkę", CreatePackage),
            ['3'] = new("Zmień status paczki", UpdateStatus),
            ['0'] = new("Powrót", null),
        };

        private void ShowAllPackages()
        {
            var packages = _packageService.GetAll();
            if (!packages.Any())
            {
                System.Console.WriteLine("Brak paczek w systemie.");
                ConsoleHelpers.Pause();
                return;
            }

            foreach (var p in packages)
            {
                System.Console.WriteLine(p.ToString());
            }
            ConsoleHelpers.Pause();
        }

        private void CreatePackage()
        {
            float weight = ConsoleHelpers.ReadFloat("Podaj wagę (kg): ");
            string size = ConsoleHelpers.ReadString("Podaj rozmiar (Small/Medium/Big): ");

            var id = Guid.NewGuid();
            _packageService.CreatePackage(id, DateTime.Now, weight, size);

            System.Console.WriteLine($"Paczka nadana! Numer śledzenia: {id}");
            ConsoleHelpers.Pause();
        }

        private void UpdateStatus()
        {
            var packages = _packageService.GetAll().ToList();
            if (!packages.Any())
            {
                System.Console.WriteLine("Brak paczek.");
                ConsoleHelpers.Pause();
                return;
            }

            for (int i = 0; i < packages.Count; i++)
            {
                System.Console.WriteLine($"{i + 1}) {packages[i]}");
            }

            int idx = ConsoleHelpers.ReadIndex($"Wybierz paczkę (1..{packages.Count}): ", packages.Count);
            if (idx < 0) return;

            System.Console.WriteLine("Dostępne statusy: 0-Sent, 1-InTransit, 2-Delivered, 3-Damaged");
            int statusInt = (int)ConsoleHelpers.ReadFloat("Podaj numer statusu: ");

            if (statusInt >= 0 && statusInt <= 3)
            {
                _packageService.UpdatePackageStatus(packages[idx].TrackingNumber, (PackageStatus)statusInt);
                System.Console.WriteLine("Status zaktualizowany.");
            }
            else
            {
                System.Console.WriteLine("Niepoprawny status.");
            }
            ConsoleHelpers.Pause();
        }
    }
}