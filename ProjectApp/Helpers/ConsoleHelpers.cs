using System;
using System.Globalization;
using ProjectApp.DataModel;

namespace ProjectApp.Console.Helpers
{
    public static class ConsoleHelpers
    {
        public static void WriteColoredPackage(Package p, string suffix = "")
        {
            var (text, color) = p.PaymentStatus switch
            {
                PaymentStatus.Oplacona => ("[OPŁACONA]", ConsoleColor.Green),
                PaymentStatus.PlatnoscPrzyOdbiorze => ("[PRZY ODBIORZE]", ConsoleColor.Blue),
                _ => ("[NIEOPŁACONA]", ConsoleColor.Red)
            };

            System.Console.ForegroundColor = color;
            System.Console.Write(text + " ");
            System.Console.ResetColor();

            System.Console.WriteLine(p.ToString() + " " + suffix);
        }

        public static void WriteDeliveredPackage(Package p, string suffix = "")
        {
            var (text, color) = p.PaymentStatus switch
            {
                PaymentStatus.Oplacona => ("[OPŁACONA]", ConsoleColor.Green),
                PaymentStatus.PlatnoscPrzyOdbiorze => ("[PRZY ODBIORZE]", ConsoleColor.Blue),
                _ => ("[NIEOPŁACONA]", ConsoleColor.Red)
            };

            System.Console.ForegroundColor = color;
            System.Console.Write(text + " ");
            System.Console.ResetColor();

            System.Console.ForegroundColor = ConsoleColor.DarkGray;
            System.Console.WriteLine(p.ToString() + " " + suffix);
            System.Console.ResetColor();
        }

        public static void WriteDestroyedPackage(Package p, string suffix = "")
        {
            var (text, color) = p.PaymentStatus switch
            {
                PaymentStatus.Oplacona => ("[OPŁACONA]", ConsoleColor.Green),
                PaymentStatus.PlatnoscPrzyOdbiorze => ("[PRZY ODBIORZE]", ConsoleColor.Blue),
                _ => ("[NIEOPŁACONA]", ConsoleColor.Red)
            };

            System.Console.ForegroundColor = color;
            System.Console.Write(text + " ");
            System.Console.ResetColor();

            System.Console.ForegroundColor = ConsoleColor.DarkRed;
            System.Console.WriteLine(p.ToString() + " " + suffix);
            System.Console.ResetColor();
        }

        public static void Pause()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            System.Console.ReadKey(true);
        }

        public static int ReadIndex(string prompt, int count)
        {
            while (true)
            {
                System.Console.Write(prompt);
                var text = System.Console.ReadLine();

                if (int.TryParse(text, out int number) && number >= 1 && number <= count)
                {
                    return number - 1;
                }

                PrintError($"Wybierz liczbę z zakresu 1-{count}.");
            }
        }

        public static float ReadFloat(string prompt, float min = 0.0f, float max = float.MaxValue)
        {
            float result;
            while (true)
            {
                System.Console.Write(prompt);
                var text = System.Console.ReadLine();

                if (float.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
                {
                    if (result > min && result <= max)
                    {
                        return result;
                    }
                }

                PrintError($"Podaj poprawną liczbę (większą od {min}).");
            }
        }

        public static int ReadInt(string prompt, int min, int max)
        {
            int result;
            while (true)
            {
                System.Console.Write(prompt);
                var text = System.Console.ReadLine();

                if (int.TryParse(text, out result))
                {
                    if (result >= min && result <= max)
                    {
                        return result;
                    }
                }

                PrintError($"Podaj liczbę całkowitą z zakresu {min}-{max}.");
            }
        }

        public static string ReadString(string prompt)
        {
            while (true)
            {
                System.Console.Write(prompt);
                string? input = System.Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(input))
                {
                    return input.Trim();
                }

                PrintError("To pole nie może być puste. Wpisz wartość.");
            }
        }

        private static void PrintError(string msg)
        {
            System.Console.ForegroundColor = ConsoleColor.Red;
            System.Console.WriteLine($" >>> BŁĄD: {msg}");
            System.Console.ResetColor();
        }
    }
}