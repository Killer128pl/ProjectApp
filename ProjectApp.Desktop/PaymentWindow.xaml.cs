using ProjectApp.DataModel;
using System.Threading.Tasks;
using System.Windows;

namespace ProjectApp.Desktop
{
    public partial class PaymentWindow : Window
    {
        public PaymentStatus ResultStatus { get; private set; }

        public PaymentWindow()
        {
            InitializeComponent();
            SimulateProcess(); // Startujemy "fake'owe" ładowanie od razu po otwarciu
        }

        private async void SimulateProcess()
        {
            StatusText.Text = "Łączenie z bankiem...";
            await Task.Delay(800);
            PaymentProgress.Value = 30;

            StatusText.Text = "Autoryzacja tokena...";
            await Task.Delay(1000);
            PaymentProgress.Value = 70;

            StatusText.Text = "Weryfikacja środków...";
            await Task.Delay(800);
            PaymentProgress.Value = 100;

            StatusText.Text = "Gotowe do zatwierdzenia.";
            StatusText.Foreground = System.Windows.Media.Brushes.Green;

            ActionButtons.Visibility = Visibility.Visible;
        }

        private void Pay_Click(object sender, RoutedEventArgs e)
        {
            ResultStatus = PaymentStatus.Oplacona;
            DialogResult = true;
        }

        private void COD_Click(object sender, RoutedEventArgs e)
        {
            ResultStatus = PaymentStatus.PlatnoscPrzyOdbiorze;
            DialogResult = true;
        }
    }
}