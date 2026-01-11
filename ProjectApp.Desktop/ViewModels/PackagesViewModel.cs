using ProjectApp.DataAccess.Memory;
using ProjectApp.ServiceAbstractions;
using ProjectApp.Desktop.Infrastructure;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProjectApp.Desktop.ViewModels
{
    public class PackagesViewModel : BaseViewModel
    {
        private readonly IPackageService _packageSvc;
        private readonly MemoryDbContext _db; // Potrzebne do pobrania nazwiska klienta

        // Kolekcja, którą widzi DataGrid w oknie
        public ObservableCollection<PackageListItemViewModel> Packages { get; } = new();

        public RelayCommand LoadCommand { get; }

        public PackagesViewModel(IPackageService packageSvc, MemoryDbContext db)
        {
            _packageSvc = packageSvc;
            _db = db;

            LoadCommand = new RelayCommand(LoadPackages);

            // Załaduj dane automatycznie przy starcie
            LoadPackages();
        }

        private void LoadPackages()
        {
            Packages.Clear();
            var items = _packageSvc.GetAll();

            foreach (var p in items)
            {
                // Szukamy klienta, żeby wyświetlić nazwisko zamiast ID
                var client = _db.Clients.FirstOrDefault(c => c.ClientId == p.SenderId);
                string clientName = client != null ? $"{client.FirstName} {client.LastName}" : "Nieznany";

                Packages.Add(new PackageListItemViewModel
                {
                    TrackingNumber = p.TrackingNumber,
                    SenderName = clientName,
                    Weight = p.Weight,
                    Status = p.PackageStatus.ToString(),
                    PaymentStatus = p.PaymentStatus.ToString()
                });
            }
        }
    }
}