using System;
using System.Linq;
using System.Windows;

namespace pizza
{
    public partial class CreditCardPopup : Window
    {
        public CreditCardPopup()
        {
            InitializeComponent();
            LoadExpirationOptions();
            CardholderNameBox.Text = App.CurrentUserFullName;
        }

        ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
        public pizza.ServiceReference1.CreditCard EnteredCard { get; private set; }

        public bool ShouldSaveCard => SaveCardCheckBox.IsChecked == true;

        private void LoadExpirationOptions()
        {
            ExpMonthComboBox.ItemsSource = Enumerable.Range(1, 12)
                .Select(month => month.ToString("00"))
                .ToList();

            ExpYearComboBox.ItemsSource = Enumerable.Range(DateTime.Now.Year, 12)
                .Select(year => year.ToString())
                .ToList();

            ExpMonthComboBox.SelectedIndex = 0;
            ExpYearComboBox.SelectedIndex = 0;
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            string cardholderName = (CardholderNameBox.Text ?? string.Empty).Trim();
            string cardNumber = NormalizeDigits(CardNumberBox.Text);
            string cvv = NormalizeDigits(CvvBox.Text);
            string month = ExpMonthComboBox.SelectedItem as string;
            string year = ExpYearComboBox.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(cardholderName))
            {
                MessageBox.Show("Please enter the cardholder name.");
                return;
            }

            if (cardNumber.Length < 12 || cardNumber.Length > 19)
            {
                MessageBox.Show("Please enter a valid card number.");
                return;
            }

            if (string.IsNullOrWhiteSpace(month) || string.IsNullOrWhiteSpace(year))
            {
                MessageBox.Show("Please select the expiration date.");
                return;
            }

            if (cvv.Length < 3 || cvv.Length > 4)
            {
                MessageBox.Show("Please enter a valid CVV.");
                return;
            }

            EnteredCard = new pizza.ServiceReference1.CreditCard
            {
                UserId = App.CurrentUserId,
                Name = cardholderName,
                Number = cardNumber,
                ExpDate = $"{month}/{year}",
                CVV = cvv
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private static string NormalizeDigits(string value)
        {
            return new string((value ?? string.Empty).Where(char.IsDigit).ToArray());
        }
    }
}
