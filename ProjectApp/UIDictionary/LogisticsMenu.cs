using ProjectApp.Console.Helpers;
using ProjectApp.DataAccess.Memory;
using ProjectApp.Services;
using System.Linq; // Upewnij się, że masz ten using

namespace ProjectApp.Console.UIDictionary
{
    public class LogisticsMenu : MenuBase
    {
        private readonly LogisticsService _logisticsService;
        private readonly MemoryDbContext _db;

        public LogisticsMenu(LogisticsService logisticsService, MemoryDbContext db)
        {
            _logisticsService = logisticsService;
            _db = db;
        }

        protected override string Title => "Logistyka i flota";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Pokaż pracowników", ShowWorkers),
            ['2'] = new("Pokaż pojazdy", ShowVehicles),
            ['3'] = new("Przypisz pojazd do kuriera", AssignVehicle),
            ['4'] = new("Wydaj paczkę kurierowi", AssignPackage),
            ['0'] = new("Powrót", null),
        };

        private void ShowWorkers()
        {
            System.Console.Clear();
            if (!_db.Workers.Any()) System.Console.WriteLine("Brak pracowników.");
            foreach (var w in _db.Workers)
            {
                if (w.Position == "Kurier") {
                    string autoInfo = w.AssignedVehicleId.HasValue ?
                        $"AutoID: {w.AssignedVehicleId}" : "Brak auta";
                    System.Console.WriteLine($"{w.FirstName} {w.LastName} [{w.Position}] - {autoInfo}");
                }
                else
                {
                    System.Console.WriteLine($"{w.FirstName} {w.LastName} [{w.Position}]");
                }
            }
            ConsoleHelpers.Pause();
        }

        private void ShowVehicles()
        {
            System.Console.Clear();
            if (!_db.Vehicles.Any()) System.Console.WriteLine("Brak pojazdów.");
            foreach (var v in _db.Vehicles)
            {
                System.Console.WriteLine($"{v.Brand} {v.Model} ({v.RegNumber}) - Status: {v.VehicleStatus}");
            }
            ConsoleHelpers.Pause();
        }

        private void AssignVehicle()
        {
            System.Console.Clear();

            var workers = _db.Workers.Where(w => w.Position == "Kurier").ToList();
            var vehicles = _db.Vehicles.ToList();

            if (!workers.Any())
            {
                System.Console.WriteLine("Brak kurierów, którym można przypisać pojazd.");
                ConsoleHelpers.Pause();
                return;
            }
            if (!vehicles.Any())
            {
                System.Console.WriteLine("Brak pojazdów w bazie.");
                ConsoleHelpers.Pause();
                return;
            }

            System.Console.WriteLine("--- Kurierzy ---");
            for (int i = 0; i < workers.Count; i++)
                System.Console.WriteLine($"{i + 1}) {workers[i].LastName} {workers[i].FirstName}");

            int wIdx = ConsoleHelpers.ReadIndex("Wybierz pracownika: ", workers.Count);

            System.Console.WriteLine("\n--- Pojazdy ---");
            for (int i = 0; i < vehicles.Count; i++)
                System.Console.WriteLine($"{i + 1}) {vehicles[i].Brand} {vehicles[i].Model} [{vehicles[i].VehicleStatus}]");

            int vIdx = ConsoleHelpers.ReadIndex("Wybierz pojazd: ", vehicles.Count);

            bool success = _logisticsService.AssignVehicleToWorker(workers[wIdx].WorkerId, vehicles[vIdx].VehicleId);
            System.Console.WriteLine(success ? "Przypisano pojazd." : "Błąd przypisywania (pojazd zajęty lub złe dane).");
            ConsoleHelpers.Pause();
        }

        private void AssignPackage()
        {
            System.Console.Clear();
            var packages = _db.Packages.Where(p => p.PackageStatus == DataModel.PackageStatus.Nadana).ToList();

            var couriers = _db.Workers.Where(w => w.Position == "Kurier").ToList();

            if (!packages.Any())
            {
                System.Console.WriteLine("Brak paczek oczekujących na wydanie.");
                ConsoleHelpers.Pause();
                return;
            }
            if (!couriers.Any())
            {
                System.Console.WriteLine("Brak kurierów.");
                ConsoleHelpers.Pause();
                return;
            }

            System.Console.WriteLine("--- Nadane Paczki ---");
            for (int i = 0; i < packages.Count; i++)
            {
                var client = _db.Clients.FirstOrDefault(c => c.ClientId == packages[i].SenderId);
                string senderInfo = client != null ? $"{client.LastName} {client.FirstName}" : "Nieznany";

                System.Console.Write($"{i + 1}) ");

                ConsoleHelpers.WriteColoredPackage(packages[i], $"[Nadawca: {senderInfo}]");
            }

            int pIdx = ConsoleHelpers.ReadIndex("Wybierz paczkę: ", packages.Count);

            System.Console.WriteLine("\n--- Kurierzy ---");
            for (int i = 0; i < couriers.Count; i++)
            {
                string auto = couriers[i].AssignedVehicleId.HasValue ?
                    "Ma przypisane auto" : "Pieszy";
                System.Console.WriteLine($"{i + 1}) {couriers[i].LastName} {couriers[i].FirstName} [{auto}]");
            }

            int cIdx = ConsoleHelpers.ReadIndex("Wybierz kuriera: ", couriers.Count);

            bool success = _logisticsService.AssignPackageToCourier(packages[pIdx].TrackingNumber, couriers[cIdx].WorkerId);
            System.Console.WriteLine(success ? "Paczka wydana kurierowi." : "Błąd: Kurier musi mieć przypisane auto!");
            ConsoleHelpers.Pause();
        }
    }
}