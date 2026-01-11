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
    }
}