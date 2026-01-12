using ProjectApp.DataAccess.Memory;
using ProjectApp.DataModel;
using ProjectApp.Desktop.Infrastructure;
using ProjectApp.ServiceAbstractions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace ProjectApp.Desktop.ViewModels
{
    public class PackagesViewModel : BaseViewModel
    {
        private readonly IPackageService _packageSvc;
        private readonly MemoryDbContext _db;

        public ObservableCollection<PackageListItemViewModel> Packages { get; } = new();

        public ObservableCollection<Client> AvailableClients { get; } = new();

        private PackageListItemViewModel? _selectedPackage;
        public PackageListItemViewModel? SelectedPackage
        {
            get => _selectedPackage;
            set
            {
                if (Set(ref _selectedPackage, value))
                {
                    DeleteCommand.RaiseCanExecuteChanged();
                    NextStatusCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private Client? _selectedSender;
        public Client? SelectedSender
        {
            get => _selectedSender;
            set => Set(ref _selectedSender, value);
        }

        private float _newWeight;
        public float NewWeight { get => _newWeight; set => Set(ref _newWeight, value); }

        private string _newSize = string.Empty;
        public string NewSize { get => _newSize; set => Set(ref _newSize, value); }

        public RelayCommand LoadCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand NextStatusCommand { get; }

        public PackagesViewModel(IPackageService packageSvc, MemoryDbContext db)
        {
            _packageSvc = packageSvc;
            _db = db;

            LoadCommand = new RelayCommand(LoadData);
            AddCommand = new RelayCommand(AddPackage);
            DeleteCommand = new RelayCommand(DeletePackage, () => SelectedPackage != null);
            NextStatusCommand = new RelayCommand(OpenStatusDialog, () => SelectedPackage != null);

            NewWeight = 1.0f;
            NewSize = "Mała";

            LoadData();
        }

        private void LoadData()
        {
            Packages.Clear();
            var items = _packageSvc.GetAll();

            foreach (var item in items)
            {
                var client = _db.Clients.FirstOrDefault(c => c.ClientId == item.SenderId);
                string clientName = client != null ? $"{client.FirstName} {client.LastName}" : "Nieznany";

                Packages.Add(new PackageListItemViewModel
                {
                    TrackingNumber = item.TrackingNumber,
                    SenderName = clientName,
                    Weight = item.Weight,
                    Size = item.Size ?? "-",
                    Status = item.PackageStatus.ToString(),
                    Payment = item.PaymentStatus.ToString()
                });
            }

            AvailableClients.Clear();
            foreach (var c in _db.Clients)
            {
                AvailableClients.Add(c);
            }

            if (SelectedSender == null && AvailableClients.Any())
            {
                SelectedSender = AvailableClients.First();
            }
        }

        private void AddPackage()
        {
            if (SelectedSender == null)
            {
                MessageBox.Show("Wybierz nadawcę z listy!");
                return;
            }

            _packageSvc.CreatePackage(
                Guid.NewGuid(),
                SelectedSender.ClientId,
                DateTime.Now,
                NewWeight,
                NewSize,
                PaymentStatus.Nieoplacona
            );

            NewWeight = 1.0f;
            NewSize = "Mała";
            LoadData();
        }

        private void DeletePackage()
        {
            if (SelectedPackage == null) return;

            if (MessageBox.Show("Na pewno usunąć tę paczkę?", "Potwierdzenie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _packageSvc.DeletePackage(SelectedPackage.TrackingNumber);
                LoadData();
            }
        }

        private void OpenStatusDialog()
        {
            if (SelectedPackage == null) return;

            if (Enum.TryParse<PackageStatus>(SelectedPackage.Status, out var currentStatus))
            {
                var dialog = new StatusWindow(currentStatus);

                dialog.Owner = Application.Current.MainWindow;

                if (dialog.ShowDialog() == true)
                {
                    var newStatus = dialog.SelectedStatus;
                    _packageSvc.UpdatePackageStatus(SelectedPackage.TrackingNumber, newStatus);
                    LoadData();
                }
            }
        }
    }
}