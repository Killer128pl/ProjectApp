using ProjectApp.Abstractions;
using ProjectApp.Console.Helpers;
using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace ProjectApp.Console.UIDictionary
{
    public class PackageMenu : MenuBase
    {
        private readonly IPackageService _packageService;
        private readonly MemoryDbContext _db;

        public PackageMenu(IPackageService packageService, MemoryDbContext db)
        {
            _packageService = packageService;
            _db = db;
        }

        protected override string Title => "Panel zarządzania paczkami (Admin)";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Lista wszystkich paczek", ShowAllPackages),
            ['2'] = new("Nadaj paczkę (dla Klienta)", CreatePackage),
            ['3'] = new("Zmień status (Logistyka)", UpdateStatus),
            ['4'] = new("Zmień status płatności", UpdatePaymentStatus),
            ['0'] = new("Powrót", null),
        };

        private void ShowAllPackages()
        {
            System.Console.Clear();
            var p = _packageService.GetAll();
            if (!p.Any())
            {
                System.Console.WriteLine("Brak paczek.");
                ConsoleHelpers.Pause();
                return;
            }

            foreach (var item in p)
            {
                var client = _db.Clients.FirstOrDefault(c => c.ClientId == item.SenderId);
                string senderInfo = client != null ? $"{client.LastName} {client.FirstName}" : "Nieznany";
                if (item.PackageStatus == PackageStatus.Dostarczona)
                {
                    ConsoleHelpers.WriteDeliveredPackage(item, $"| Nadawca: {senderInfo}");
                }
                else if (item.PackageStatus == PackageStatus.Uszkodzona)
                {
                    ConsoleHelpers.WriteDestroyedPackage(item, $"| Nadawca: {senderInfo}");
                }
                else
                {
                    ConsoleHelpers.WriteColoredPackage(item, $"| Nadawca: {senderInfo}");
                }
            }
            ConsoleHelpers.Pause();
        }

        private void CreatePackage()
        {
            System.Console.Clear();
            var clients = _db.Clients.ToList();
            if (!clients.Any()) { System.Console.WriteLine("Brak klientów."); ConsoleHelpers.Pause(); return; }

            for (int i = 0; i < clients.Count; i++)
                System.Console.WriteLine($"{i + 1}) {clients[i].FirstName} {clients[i].LastName}");

            int cIdx = ConsoleHelpers.ReadIndex("Wybierz nadawcę: ", clients.Count);

            float w = ConsoleHelpers.ReadFloat("Waga (kg): ", min: 0.1f);
            string s = ConsoleHelpers.ReadString("Rozmiar: ");

            System.Console.WriteLine("Typ płatności: 1 - Płatność ze strony klienta, 2 - Na miejscu, 3 - Przy odbiorze");
            int pType = ConsoleHelpers.ReadInt("Wybierz: ", 1, 3);

            PaymentStatus pStatus;

            if (pType == 1)
            {
                pStatus = PaymentStatus.Nieoplacona;
            }
            else if (pType == 2)
            {
                pStatus = PaymentStatus.Oplacona;
            }
            else
            {
                pStatus = PaymentStatus.PlatnoscPrzyOdbiorze;
            }

            var id = Guid.NewGuid();
            _packageService.CreatePackage(id, clients[cIdx].ClientId, DateTime.Now, w, s, pStatus);
            if (pType == 1)
            {
                System.Console.WriteLine("Utworzono. Paczka do opłacenia w panelu klienta.");
            }
            else
            {
                System.Console.WriteLine("Utworzono.");
            }
            ConsoleHelpers.Pause();
        }

        private void UpdateStatus()
        {
            System.Console.Clear();
            var packs = _packageService.GetAll().ToList();
            if (!packs.Any()) return;

            for (int i = 0; i < packs.Count; i++)
            {
                var client = _db.Clients.FirstOrDefault(c => c.ClientId == packs[i].SenderId);
                string? sender = client != null ? client.LastName : "---";

                System.Console.Write($"{i + 1}) ");
                ConsoleHelpers.WriteColoredPackage(packs[i], $"[Nadawca: {sender}]");
            }

            int idx = ConsoleHelpers.ReadIndex("Wybierz: ", packs.Count);

            System.Console.WriteLine("0 - Nadana, 1 - W Trasie, 2 - Dostarczona, 3 - Uszkodzona");
            int st = ConsoleHelpers.ReadInt("Status: ", 0, 3);

            _packageService.UpdatePackageStatus(packs[idx].TrackingNumber, (PackageStatus)st);
            ConsoleHelpers.Pause();
        }

        private void UpdatePaymentStatus()
        {
            System.Console.Clear();
            var packs = _packageService.GetAll().ToList();
            for (int i = 0; i < packs.Count; i++)
            {
                var client = _db.Clients.FirstOrDefault(c => c.ClientId == packs[i].SenderId);
                string? sender = client != null ? client.LastName : "---";

                System.Console.Write($"{i + 1}) ");
                ConsoleHelpers.WriteColoredPackage(packs[i], $"[Nadawca: {sender}]");
            }

            int idx = ConsoleHelpers.ReadIndex("Wybierz: ", packs.Count);

            System.Console.WriteLine("0 - Oplacona, 1 - Nieoplacona, 2 - Płatność Przy Odbiorze");
            int st = ConsoleHelpers.ReadInt("Status: ", 0, 2);

            _packageService.UpdatePaymentStatus(packs[idx].TrackingNumber, (PaymentStatus)st);
            ConsoleHelpers.Pause();
        }
    }
}