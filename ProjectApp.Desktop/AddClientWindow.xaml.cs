using System.Windows;

namespace ProjectApp.Desktop
{
    public partial class AddClientWindow : Window
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public long PhoneNumber { get; private set; }

        public AddClientWindow()
        {
            InitializeComponent();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) || string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                MessageBox.Show("Podaj imię i nazwisko!");
                return;
            }

            if (!long.TryParse(PhoneBox.Text, out long phone))
            {
                MessageBox.Show("Numer telefonu musi składać się z cyfr!");
                return;
            }

            FirstName = FirstNameBox.Text;
            LastName = LastNameBox.Text;
            PhoneNumber = phone;

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}