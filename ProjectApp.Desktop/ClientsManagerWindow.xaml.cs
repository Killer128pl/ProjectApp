using Microsoft.EntityFrameworkCore;
using ProjectApp.DataAccess.Database;
using ProjectApp.DataModel;
using System.Linq;
using System.Windows;

namespace ProjectApp.Desktop
{
    public partial class ClientsManagerWindow : Window
    {
        private readonly DatabaseDbContext _db;

        public ClientsManagerWindow(DatabaseDbContext db)
        {
            InitializeComponent();
            _db = db;
            LoadClients();
        }

        private void LoadClients()
        {
            ClientsGrid.ItemsSource = _db.Clients.ToList();
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var addWindow = new AddClientWindow();
            addWindow.Owner = this;

            if (addWindow.ShowDialog() == true)
            {
                var newClient = new Client
                {
                    ClientId = System.Guid.NewGuid(),
                    FirstName = addWindow.FirstName,
                    LastName = addWindow.LastName,
                    PhoneNumber = (int)addWindow.PhoneNumber
                };

                _db.Clients.Add(newClient);
                _db.SaveChanges();
                LoadClients();
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            var selectedClient = ClientsGrid.SelectedItem as Client;
            if (selectedClient == null) return;

            bool hasPackages = _db.Packages.Any(p => p.SenderId == selectedClient.ClientId);

            if (hasPackages)
            {
                MessageBox.Show("Nie można usunąć tego klienta, ponieważ ma przypisane paczki w systemie.\n\nNajpierw usuń jego paczki.",
                    "Błąd usuwania", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (MessageBox.Show($"Czy na pewno usunąć klienta: {selectedClient.FirstName} {selectedClient.LastName}?",
                "Potwierdzenie", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _db.Clients.Remove(selectedClient);
                _db.SaveChanges();
                LoadClients();
            }
        }


        private void Close_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }
    }
}