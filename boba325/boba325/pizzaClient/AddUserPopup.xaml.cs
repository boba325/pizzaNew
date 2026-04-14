using System.Windows;

namespace pizza
{
    public partial class AddUserPopup : Window
    {
        public AddUserPopup()
        {
            InitializeComponent();
        }

        ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
        public pizza.ServiceReference1.User CreatedUser { get; private set; }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Text))
            {
                MessageBox.Show("מלא את השדות הנדרשים.");
                return;
            }

            CreatedUser = new pizza.ServiceReference1.User
            {
                FName = FirstNameBox.Text.Trim(),
                LName = LastNameBox.Text.Trim(),
                Email = EmailBox.Text.Trim(),
                Password = PasswordBox.Text.Trim(),
                Phone = (PhoneBox.Text ?? string.Empty).Trim(),
                Address = (AddressBox.Text ?? string.Empty).Trim(),
                Bday = (BirthdayBox.Text ?? string.Empty).Trim(),
                IsAdmin = IsAdminCheckBox.IsChecked == true
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
