using ProjectApp.Console.Helpers;
using System;
using System.Threading;

namespace ProjectApp.Console.UIDictionary
{
    public static class PaymentSimulator
    {
        public static bool ProcessPayment(float amount)
        {
            System.Console.Clear();
            System.Console.WriteLine("========================================");
            System.Console.WriteLine("          BRAMKA PŁATNOŚCI 2.0          ");
            System.Console.WriteLine("========================================");
            System.Console.WriteLine($"Kwota do zapłaty: {amount:C2}");
            System.Console.WriteLine("\nWybierz metodę płatności:");
            System.Console.WriteLine("1. BLIK");
            System.Console.WriteLine("2. Karta Kredytowa");
            System.Console.WriteLine("3. Przelew Tradycyjny");
            System.Console.WriteLine("0. Anuluj");

            System.Console.Write("\nWybór: ");
            var key = System.Console.ReadKey(true);

            if (key.KeyChar == '0') return false;

            System.Console.WriteLine("\n\nŁączenie z bankiem...");
            for (int i = 0; i < 8; i++)
            {
                System.Console.Write("█");
                Thread.Sleep(300);
            }

            System.Console.ForegroundColor = ConsoleColor.Green;
            System.Console.WriteLine("\n\nPŁATNOŚĆ ZAAKCEPTOWANA!");
            System.Console.ResetColor();
            System.Console.WriteLine("Dziękujemy za skorzystanie z usług.");

            ConsoleHelpers.Pause();
            return true;
        }
    }
}