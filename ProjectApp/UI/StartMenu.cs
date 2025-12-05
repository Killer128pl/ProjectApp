using ProjectApp.Console.Helpers;
using ProjectApp.DataAccess.Memory;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Console.UI
{
    public class StartMenu : MenuBase
    {
        private readonly IPackageService _packageSvc;
        private readonly LogisticsService _logisticsSvc;
        private readonly MemoryDbContext _db;
        private readonly MainMenu _adminPanel;

        public StartMenu(IPackageService pkg, LogisticsService log, MemoryDbContext db)
        {
            _packageSvc = pkg;
            _logisticsSvc = log;
            _db = db;
            _adminPanel = new MainMenu(pkg, log, db);
        }

        protected override string Title => "SYSTEM LOGISTYCZNY - LOGOWANIE";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Panel Administratora", () => _adminPanel.Run()),
            ['2'] = new("Panel Klienta", RunClientLogin),
            ['3'] = new("Panel Pracownika", RunWorkerLogin),
            ['0'] = new("Wyjście z systemu", null)
        };

        private void RunClientLogin()
        {
            var clients = _db.Clients.ToList();
            if (!clients.Any())
            {
                var c = new DataModel.Client { FirstName = "Jan", LastName = "Kowalski" };
                _db.Clients.Add(c);
                clients.Add(c);
            }

            System.Console.WriteLine("--- WYBIERZ UŻYTKOWNIKA ---");
            for (int i = 0; i < clients.Count; i++)
                System.Console.WriteLine($"{i + 1}) {clients[i].FirstName} {clients[i].LastName} (ID: ...{clients[i].ClientId.ToString().Substring(30)})");

            int idx = ConsoleHelpers.ReadIndex("Loguj jako: ", clients.Count);
            if (idx < 0) return;

            new ClientMenu(_packageSvc, clients[idx]).Run();
        }

        private void RunWorkerLogin()
        {
            var workers = _db.Workers.ToList();
            if (!workers.Any())
            {
                System.Console.WriteLine("Brak pracowników w bazie.");
                ConsoleHelpers.Pause();
                return;
            }

            System.Console.WriteLine("--- WYBIERZ PRACOWNIKA ---");
            for (int i = 0; i < workers.Count; i++)
                System.Console.WriteLine($"{i + 1}) {workers[i].FirstName} {workers[i].LastName} [{workers[i].Position}]");

            int idx = ConsoleHelpers.ReadIndex("Loguj jako: ", workers.Count);
            if (idx < 0) return;

            new WorkerMenu(_packageSvc, _db, workers[idx]).Run();
        }
    }
}