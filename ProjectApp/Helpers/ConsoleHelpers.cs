using System.Globalization;

namespace ProjectApp.Console.Helpers
{
    public static class ConsoleHelpers
    {
        public static void Pause()
        {
            System.Console.WriteLine();
            System.Console.WriteLine("Naciśnij dowolny klawisz, aby kontynuować...");
            System.Console.ReadKey(true);
        }

        public static int ReadIndex(string prompt, int count)
        {
            System.Console.Write(prompt);
            var text = System.Console.ReadLine();

            if (!int.TryParse(text, out int number))
                return -1;

            if (number < 1 || number > count)
                return -1;

            return number - 1;
        }

        public static float ReadFloat(string prompt)
        {
            System.Console.Write(prompt);
            var text = System.Console.ReadLine();
            if (float.TryParse(text, NumberStyles.Any, CultureInfo.InvariantCulture, out float result))
            {
                return result;
            }
            return 0.0f;
        }

        public static string ReadString(string prompt)
        {
            System.Console.Write(prompt);
            return System.Console.ReadLine() ?? string.Empty;
        }
    }
}