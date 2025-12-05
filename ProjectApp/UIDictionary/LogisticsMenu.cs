using ProjectApp.Console.Helpers;
using ProjectApp.DataAccess.Memory;
using ProjectApp.Services;

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

        protected override string Title => "LOGISTYKA I FLOTA";
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
            if (!_db.Workers.Any()) System.Console.WriteLine("Brak pracowników.");
            foreach (var w in _db.Workers)
            {
                string autoInfo = w.AssignedVehicleId.HasValue ? $"AutoID: {w.AssignedVehicleId}" : "BRAK AUTA";
                System.Console.WriteLine($"{w.FirstName} {w.LastName} [{w.Position}] - {autoInfo}");
            }
            ConsoleHelpers.Pause();
        }

        private void ShowVehicles()
        {
            if (!_db.Vehicles.Any()) System.Console.WriteLine("Brak pojazdów.");
            foreach (var v in _db.Vehicles)
            {
                System.Console.WriteLine($"{v.Brand} {v.Model} ({v.RegNumber}) - Status: {v.VehicleStatus}");
            }
            ConsoleHelpers.Pause();
        }

        private void AssignVehicle()
        {
            var workers = _db.Workers.ToList();
            var vehicles = _db.Vehicles.ToList();

            if (!workers.Any() || !vehicles.Any())
            {
                System.Console.WriteLine("Brak danych do operacji.");
                ConsoleHelpers.Pause();
                return;
            }

            System.Console.WriteLine("--- PRACOWNICY ---");
            for (int i = 0; i < workers.Count; i++)
                System.Console.WriteLine($"{i + 1}) {workers[i].LastName} ({workers[i].Position})");

            int wIdx = ConsoleHelpers.ReadIndex("Wybierz pracownika: ", workers.Count);
            if (wIdx < 0) return;

            System.Console.WriteLine("\n--- POJAZDY ---");
            for (int i = 0; i < vehicles.Count; i++)
                System.Console.WriteLine($"{i + 1}) {vehicles[i].Brand} {vehicles[i].Model} [{vehicles[i].VehicleStatus}]");

            int vIdx = ConsoleHelpers.ReadIndex("Wybierz pojazd: ", vehicles.Count);
            if (vIdx < 0) return;

            bool success = _logisticsService.AssignVehicleToWorker(workers[wIdx].WorkerId, vehicles[vIdx].VehicleId);
            System.Console.WriteLine(success ? "Przypisano pojazd." : "Błąd przypisywania (pojazd zajęty lub złe dane).");
            ConsoleHelpers.Pause();
        }

        private void AssignPackage()
        {
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

            System.Console.WriteLine("--- PACZKI (SENT) ---");
            for (int i = 0; i < packages.Count; i++)
                System.Console.WriteLine($"{i + 1}) {packages[i].TrackingNumber} ({packages[i].Weight}kg)");

            int pIdx = ConsoleHelpers.ReadIndex("Wybierz paczkę: ", packages.Count);
            if (pIdx < 0) return;

            System.Console.WriteLine("\n--- KURIERZY ---");
            for (int i = 0; i < couriers.Count; i++)
            {
                string auto = couriers[i].AssignedVehicleId.HasValue ? "MA AUTO" : "PIESZY";
                System.Console.WriteLine($"{i + 1}) {couriers[i].LastName} [{auto}]");
            }

            int cIdx = ConsoleHelpers.ReadIndex("Wybierz kuriera: ", couriers.Count);
            if (cIdx < 0) return;

            bool success = _logisticsService.AssignPackageToCourier(packages[pIdx].TrackingNumber, couriers[cIdx].WorkerId);
            System.Console.WriteLine(success ? "Paczka wydana kurierowi." : "Błąd: Kurier musi mieć przypisane auto!");
            ConsoleHelpers.Pause();
        }
    }
}