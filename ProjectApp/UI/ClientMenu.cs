using ProjectApp.Console.Helpers;
using ProjectApp.DataModel;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ProjectApp.Console.UI
{
    public class ClientMenu : MenuBase
    {
        private readonly IPackageService _packageService;
        private readonly Client _currentClient;

        public ClientMenu(IPackageService packageService, Client client)
        {
            _packageService = packageService;
            _currentClient = client;
        }

        protected override string Title => $"PANEL KLIENTA: {_currentClient.FirstName} {_currentClient.LastName}";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Moje przesyłki", ShowMyPackages),
            ['2'] = new("Nadaj paczkę", SendPackage),
            ['3'] = new("Centrum Płatności", PayForPackages),
            ['0'] = new("Wyloguj", null),
        };

        private void ShowMyPackages()
        {
            var packages = _packageService.GetPackagesByClient(_currentClient.ClientId);
            if (!packages.Any())
            {
                System.Console.WriteLine("Brak historii przesyłek.");
                ConsoleHelpers.Pause();
                return;
            }

            foreach (var p in packages)
            {
                System.Console.WriteLine(p);
            }
            ConsoleHelpers.Pause();
        }

        private void SendPackage()
        {
            System.Console.WriteLine("--- NOWA PRZESYŁKA ---");
            float weight = ConsoleHelpers.ReadFloat("Podaj wagę (kg): ");
            string size = ConsoleHelpers.ReadString("Podaj rozmiar (Small/Medium/Big): ");

            var id = Guid.NewGuid();
            _packageService.CreatePackage(id, _currentClient.ClientId, DateTime.Now, weight, size);

            System.Console.WriteLine($"\nSukces! Paczka o numerze {id} została nadana.");
            System.Console.WriteLine("Status płatności: NIEOPŁACONA. Przejdź do płatności, aby zrealizować wysyłkę.");
            ConsoleHelpers.Pause();
        }

        private void PayForPackages()
        {
            var unpaid = _packageService.GetPackagesByClient(_currentClient.ClientId)
                                        .Where(p => p.PaymentStatus == PaymentStatus.NotPaid)
                                        .ToList();

            if (!unpaid.Any())
            {
                System.Console.WriteLine("Wszystkie Twoje rachunki są uregulowane.");
                ConsoleHelpers.Pause();
                return;
            }

            System.Console.WriteLine("--- OCZEKUJĄCE PŁATNOŚCI ---");
            for (int i = 0; i < unpaid.Count; i++)
            {
                decimal price = (decimal)unpaid[i].Weight * 10m; // liczenie ceny wysyłki
                System.Console.WriteLine($"{i + 1}) Paczka: {unpaid[i].TrackingNumber} | Do zapłaty: {price:C2}");
            }

            int idx = ConsoleHelpers.ReadIndex("\nWybierz paczkę do opłacenia (numer): ", unpaid.Count);
            if (idx < 0) return;

            float amount = unpaid[idx].Weight * 10.0f;
            bool success = PaymentSimulator.ProcessPayment(amount);

            if (success)
            {
                _packageService.UpdatePaymentStatus(unpaid[idx].TrackingNumber, PaymentStatus.Paid);
            }
        }
    }
}