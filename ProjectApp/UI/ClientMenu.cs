using ProjectApp.Console.Helpers;
using ProjectApp.Console.UIDictionary;
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

        protected override string Title => $"Panel klienta: {_currentClient.FirstName} {_currentClient.LastName}";

        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Moje przesyłki", ShowMyPackages),
            ['2'] = new("Nadaj paczkę", SendPackage),
            ['3'] = new("Centrum Płatności", PayForPackages),
            ['0'] = new("Wyloguj", null),
        };

        private void ShowMyPackages()
        {
            System.Console.Clear();
            var packages = _packageService.GetPackagesByClient(_currentClient.ClientId);
            if (!packages.Any()) { System.Console.WriteLine("Brak historii przesyłek."); ConsoleHelpers.Pause(); return; }

            foreach (var p in packages)
            {
                ConsoleHelpers.WriteColoredPackage(p);

                if (p.PackageStatus == PackageStatus.Uszkodzona)
                {
                    System.Console.ForegroundColor = ConsoleColor.Red;
                    System.Console.WriteLine("   (!) UWAGA: Paczka uszkodzona. Prosimy zgłosić się po zwrot pieniędzy.");
                    System.Console.ResetColor();
                }
            }
            ConsoleHelpers.Pause();
        }

        private void SendPackage()
        {
            System.Console.Clear();
            System.Console.WriteLine("--- Nowa przesyłka ---");

            float weight = ConsoleHelpers.ReadFloat("Podaj wagę (kg): ", min: 0.1f);
            string size = ConsoleHelpers.ReadString("Podaj rozmiar: ");

            System.Console.WriteLine("\nForma płatności:");
            System.Console.WriteLine("1. Płacę teraz (Online)");
            System.Console.WriteLine("2. Płatność przy odbiorze (Za pobraniem)");

            int payOption = ConsoleHelpers.ReadInt("Twój wybór: ", 1, 2);

            PaymentStatus status = PaymentStatus.Nieoplacona;

            if (payOption == 2)
            {
                status = PaymentStatus.PlatnoscPrzyOdbiorze;
            }
            else
            {
                System.Console.Clear();
                float cost = weight * 10.0f;
                bool paid = PaymentSimulator.ProcessPayment(cost);
                if (paid) status = PaymentStatus.Oplacona;
                else status = PaymentStatus.Nieoplacona;
            }
            System.Console.Clear();

            var id = Guid.NewGuid();
            _packageService.CreatePackage(id, _currentClient.ClientId, DateTime.Now, weight, size, status);

            System.Console.WriteLine($"\nNadano! Nr: {id}.");
            ConsoleHelpers.Pause();
        }

        private void PayForPackages()
        {
            System.Console.Clear();
            var unpaid = _packageService.GetPackagesByClient(_currentClient.ClientId)
                                        .Where(p => p.PaymentStatus == PaymentStatus.Nieoplacona)
                                        .ToList();

            if (!unpaid.Any())
            {
                System.Console.WriteLine("Wszystkie rachunki uregulowane.");
                ConsoleHelpers.Pause();
                return;
            }

            for (int i = 0; i < unpaid.Count; i++)
            {
                System.Console.Write($"{i + 1}) ");
                ConsoleHelpers.WriteColoredPackage(unpaid[i], $"Cena: {unpaid[i].Weight * 10:C}");
            }

            int idx = ConsoleHelpers.ReadIndex("Wybierz paczkę do opłacenia: ", unpaid.Count);

            if (PaymentSimulator.ProcessPayment(unpaid[idx].Weight * 10.0f))
            {
                _packageService.UpdatePaymentStatus(unpaid[idx].TrackingNumber, PaymentStatus.Oplacona);
            }
        }
    }
}