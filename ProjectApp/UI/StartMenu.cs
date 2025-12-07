using ProjectApp.Console.Helpers;
using ProjectApp.Console.UIDictionary;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Services;
using System;
using System.Linq;

namespace ProjectApp.Console.UI
{
    public class StartMenu : MenuBase
    {
        private readonly IPackageService _packageSvc;
        private readonly LogisticsService _logisticsSvc;
        private readonly MemoryDbContext _db;
        private readonly AdminMenu _adminPanel;

        public StartMenu(IPackageService pkg, LogisticsService log, MemoryDbContext db)
        {
            _packageSvc = pkg;
            _logisticsSvc = log;
            _db = db;
            _adminPanel = new AdminMenu(pkg, log, db);
        }

        protected override string Title => "System zarządzania firmą kurierską - Witamy";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Zaloguj: Administrator", () => _adminPanel.Run()),
            ['2'] = new("Zaloguj: Klient", RunClientLogin),
            ['3'] = new("Zaloguj: Pracownik", RunWorkerLogin),
            ['4'] = new("Zarejestruj nowe konto klienta", RegisterClient),
            ['0'] = new("Wyjście", null)
        };

        private void RegisterClient()
        {
            System.Console.Clear();
            System.Console.WriteLine("--- Rejestracja ---");
            string imie = ConsoleHelpers.ReadString("Podaj imię: ");
            string nazwisko = ConsoleHelpers.ReadString("Podaj nazwisko: ");

            var newClient = new Client { FirstName = imie, LastName = nazwisko };
            _db.Clients.Add(newClient);

            System.Console.WriteLine($"\nKonto utworzone! Witaj {imie}. Możesz się teraz zalogować.");
            ConsoleHelpers.Pause();
        }

        private void RunClientLogin()
        {
            System.Console.Clear();
            var clients = _db.Clients.ToList();
            if (!clients.Any())
            {
                System.Console.WriteLine("Brak zarejestrowanych kont. Zarejestruj się (Opcja 4).");
                ConsoleHelpers.Pause();
                return;
            }

            System.Console.WriteLine("--- Wybierz użytkownika ---");
            for (int i = 0; i < clients.Count; i++)
                System.Console.WriteLine($"{i + 1}) {clients[i].FirstName} {clients[i].LastName}");

            int idx = ConsoleHelpers.ReadIndex("Loguj jako: ", clients.Count);

            new ClientMenu(_packageSvc, clients[idx]).Run();
        }

        private void RunWorkerLogin()
        {
            System.Console.Clear();
            var workers = _db.Workers.ToList();
            if (!workers.Any()) { System.Console.WriteLine("Brak pracowników. Poproś Admina o dodanie konta."); ConsoleHelpers.Pause(); return; }

            System.Console.WriteLine("--- Wybierz pracownika ---");
            for (int i = 0; i < workers.Count; i++)
                System.Console.WriteLine($"{i + 1}) {workers[i].FirstName} {workers[i].LastName} [{workers[i].Position}]");

            int idx = ConsoleHelpers.ReadIndex("Loguj jako: ", workers.Count);

            var worker = workers[idx];

            if (worker.Position == "Kurier")
            {
                new WorkerMenu(_packageSvc, _db, worker).Run();
            }
            else
            {
                new WarehouseMenu(_packageSvc, _logisticsSvc, _db, worker).Run();
            }
        }
    }
}