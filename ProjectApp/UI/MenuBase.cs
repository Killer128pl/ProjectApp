using ProjectApp.Console.Helpers;

namespace ProjectApp.Console.UI
{
    public abstract class MenuBase
    {
        protected abstract string Title { get; }
        protected abstract Dictionary<char, MenuOption> Options { get; }

        public void Run()
        {
            while (true)
            {
                System.Console.Clear();
                System.Console.WriteLine($"=== {Title} ===\n");

                foreach (var kv in Options)
                    System.Console.WriteLine($"{kv.Key}) {kv.Value.Description}");

                System.Console.Write("\nWybierz opcję: ");
                var key = System.Console.ReadKey();
                System.Console.WriteLine();

                if (!Options.ContainsKey(key.KeyChar))
                {
                    System.Console.WriteLine("Nieznana opcja.");
                    ConsoleHelpers.Pause();
                    continue;
                }

                var option = Options[key.KeyChar];
                if (option.Action == null)
                    return;

                option.Action();
            }
        }
    }
}