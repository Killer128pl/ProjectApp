using ProjectApp.DataAccess.Memory;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Services;

namespace ProjectApp.Console.UI
{
    public class MainMenu : MenuBase
    {
        private readonly PackageMenu _packageMenu;
        private readonly LogisticsMenu _logisticsMenu;

        public MainMenu(IPackageService packageSvc, LogisticsService logisticsSvc, MemoryDbContext db)
        {
            _packageMenu = new PackageMenu(packageSvc);
            _logisticsMenu = new LogisticsMenu(logisticsSvc, db);
        }

        protected override string Title => "SYSTEM LOGISTYCZNY - MENU GŁÓWNE";
        protected override Dictionary<char, MenuOption> Options => new()
        {
            ['1'] = new("Obsługa Paczek", () => _packageMenu.Run()),
            ['2'] = new("Logistyka (Flota i Kurierzy)", () => _logisticsMenu.Run()),
            ['0'] = new("Wyjście z systemu", null),
        };
    }
}