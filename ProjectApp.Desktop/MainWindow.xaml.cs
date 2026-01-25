using System.Windows;
using ProjectApp.Desktop.ViewModels;

namespace ProjectApp.Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow(PackagesViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
        private void Close_Click(object sender, RoutedEventArgs e) => Close();
    }
}