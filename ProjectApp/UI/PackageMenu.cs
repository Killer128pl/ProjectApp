using ProjectApp.Abstractions;
using ProjectApp.Console.Helpers;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Console.UI
{
    public class PackageMenu : MenuBase
    {
        private readonly IPackageService _packageService;
        private readonly MemoryDbContext _db; // Potrzebne, żeby wybrać klienta

        public PackageMenu(IPackageService packageService, MemoryDbContext db)
        {
            _packageService = packageService;
            _db = db;
        }

        protected override string Title => "ZARZĄDZANIE PACZKAMI (ADMIN)";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Lista wszystkich paczek", ShowAllPackages),
            ['2'] = new("Nadaj nową paczkę (dla Klienta)", CreatePackage),
            ['3'] = new("Zmień status paczki (Logistyczny)", UpdateStatus),
            ['4'] = new("Zmień status płatności (Księgowy)", UpdatePaymentStatus),
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

            var clients = _db.Clients.ToList();
            if (!clients.Any())
            {
                System.Console.WriteLine("Brak klientów w bazie. Najpierw dodaj klienta.");
                ConsoleHelpers.Pause();
                return;
            }

            System.Console.WriteLine("--- WYBIERZ NADAWCĘ ---");
            for (int i = 0; i < clients.Count; i++)
            {
                System.Console.WriteLine($"{i + 1}) {clients[i].FirstName} {clients[i].LastName}");
            }
            int cIdx = ConsoleHelpers.ReadIndex("Wybierz klienta: ", clients.Count);
            if (cIdx < 0) return;

            var selectedClient = clients[cIdx];

            System.Console.WriteLine($"\nNadawanie dla: {selectedClient.FirstName} {selectedClient.LastName}");
            float weight = ConsoleHelpers.ReadFloat("Podaj wagę (kg): ");
            string size = ConsoleHelpers.ReadString("Podaj rozmiar (Small/Medium/Big): ");

            var id = Guid.NewGuid();

            _packageService.CreatePackage(id, selectedClient.ClientId, DateTime.Now, weight, size);

            System.Console.WriteLine($"Paczka utworzona! Numer śledzenia: {id}");
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
                System.Console.WriteLine($"{i + 1}) {packages[i]}");

            int idx = ConsoleHelpers.ReadIndex($"Wybierz paczkę (1..{packages.Count}): ", packages.Count);
            if (idx < 0) return;

            System.Console.WriteLine("Dostępne statusy: 0-Sent, 1-InTransit, 2-Delivered, 3-Damaged");
            int statusInt = (int)ConsoleHelpers.ReadFloat("Podaj numer statusu: ");

            if (statusInt >= 0 && statusInt <= 3)
            {
                _packageService.UpdatePackageStatus(packages[idx].TrackingNumber, (PackageStatus)statusInt);
                System.Console.WriteLine("Status logistyczny zaktualizowany.");
            }
            else
            {
                System.Console.WriteLine("Niepoprawny status.");
            }
            ConsoleHelpers.Pause();
        }

        private void UpdatePaymentStatus()
        {
            var packages = _packageService.GetAll().ToList();
            if (!packages.Any())
            {
                System.Console.WriteLine("Brak paczek.");
                ConsoleHelpers.Pause();
                return;
            }

            for (int i = 0; i < packages.Count; i++)
                System.Console.WriteLine($"{i + 1}) {packages[i]}");

            int idx = ConsoleHelpers.ReadIndex("Wybierz paczkę: ", packages.Count);
            if (idx < 0) return;

            System.Console.WriteLine("Dostępne statusy: 0-Paid, 1-NotPaid, 2-OnDeliveryPayment");
            int pStatus = (int)ConsoleHelpers.ReadFloat("Wybierz status płatności: ");

            if (pStatus >= 0 && pStatus <= 2)
            {
                _packageService.UpdatePaymentStatus(packages[idx].TrackingNumber, (PaymentStatus)pStatus);
                System.Console.WriteLine("Status płatności zaktualizowany.");
            }
            else
            {
                System.Console.WriteLine("Błąd.");
            }
            ConsoleHelpers.Pause();
        }
    }
}