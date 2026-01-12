using ProjectApp.DataModel;
using System;
using System.Windows;

namespace ProjectApp.Desktop
{
    public partial class StatusWindow : Window
    {
        public PackageStatus SelectedStatus { get; private set; }

        public StatusWindow(PackageStatus currentStatus)
        {
            InitializeComponent();

            StatusComboBox.ItemsSource = Enum.GetValues(typeof(PackageStatus));

            StatusComboBox.SelectedItem = currentStatus;
        }

        private void Accept_Click(object sender, RoutedEventArgs e)
        {
            if (StatusComboBox.SelectedItem is PackageStatus status)
            {
                SelectedStatus = status;
                DialogResult = true;
            }
            else
            {
                MessageBox.Show("Wybierz status!");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}