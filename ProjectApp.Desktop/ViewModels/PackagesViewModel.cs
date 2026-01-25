using ProjectApp.DataAccess.Database;
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
        private readonly DatabaseDbContext _db;

        public ObservableCollection<PackageListItemViewModel> Packages { get; } = new();
        public ObservableCollection<Client> AvailableClients { get; } = new();

        private string _statsText = "Gotowy";
        public string StatsText { get => _statsText; set => Set(ref _statsText, value); }

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
                    ChangePaymentCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private Client? _selectedSender;
        public Client? SelectedSender { get => _selectedSender; set => Set(ref _selectedSender, value); }

        private float _newWeight = 1.0f;
        public float NewWeight { get => _newWeight; set => Set(ref _newWeight, value); }

        private string _newSize = "Mała";
        public string NewSize { get => _newSize; set => Set(ref _newSize, value); }

        public RelayCommand LoadCommand { get; }
        public RelayCommand AddCommand { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand NextStatusCommand { get; }
        public RelayCommand ManageClientsCommand { get; }
        public RelayCommand ChangePaymentCommand { get; }

        public PackagesViewModel(IPackageService packageSvc, DatabaseDbContext db)
        {
            _packageSvc = packageSvc;
            _db = db;

            LoadCommand = new RelayCommand(LoadData);
            AddCommand = new RelayCommand(AddPackage);
            DeleteCommand = new RelayCommand(DeletePackage, () => SelectedPackage != null);
            NextStatusCommand = new RelayCommand(OpenStatusDialog, () => SelectedPackage != null);
            ManageClientsCommand = new RelayCommand(OpenClientsManager);

            ChangePaymentCommand = new RelayCommand(OpenPaymentSimulator, () => SelectedPackage != null);

            LoadData();
        }

        private void LoadData()
        {
            Packages.Clear();
            var items = _packageSvc.GetAll();
            float totalWeight = 0;

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
                totalWeight += item.Weight;
            }

            StatsText = $"Liczba paczek: {items.Count}  |  Łączna waga: {totalWeight:F1} kg";

            var currentSelectedId = SelectedSender?.ClientId;
            AvailableClients.Clear();
            var clientsFromDb = _db.Clients.OrderBy(c => c.LastName).ToList();
            foreach (var c in clientsFromDb) AvailableClients.Add(c);

            if (currentSelectedId != null)
                SelectedSender = AvailableClients.FirstOrDefault(c => c.ClientId == currentSelectedId);

            if (SelectedSender == null && AvailableClients.Any())
                SelectedSender = AvailableClients.First();
        }

        private void AddPackage()
        {
            if (SelectedSender == null)
            {
                MessageBox.Show("Najpierw dodaj klienta (Menu -> Klienci)!");
                return;
            }
            _packageSvc.CreatePackage(Guid.NewGuid(), SelectedSender.ClientId, DateTime.Now, NewWeight, NewSize, PaymentStatus.Nieoplacona);
            NewWeight = 1.0f;
            NewSize = "Mała";
            LoadData();
        }

        private void OpenClientsManager()
        {
            var manager = new ClientsManagerWindow(_db);
            manager.Owner = Application.Current.MainWindow;
            manager.ShowDialog();
            LoadData();
        }

        private void OpenPaymentSimulator()
        {
            if (SelectedPackage == null) return;

            var paymentWin = new PaymentWindow();
            paymentWin.Owner = Application.Current.MainWindow;

            if (paymentWin.ShowDialog() == true)
            {
                _packageSvc.UpdatePaymentStatus(SelectedPackage.TrackingNumber, paymentWin.ResultStatus);

                LoadData();
                MessageBox.Show("Płatność zaktualizowana!");
            }
        }

        private void DeletePackage()
        {
            if (SelectedPackage == null) return;
            if (MessageBox.Show("Usunąć paczkę?", "Potwierdzenie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
                    _packageSvc.UpdatePackageStatus(SelectedPackage.TrackingNumber, dialog.SelectedStatus);
                    LoadData();
                }
            }
        }
    }
}