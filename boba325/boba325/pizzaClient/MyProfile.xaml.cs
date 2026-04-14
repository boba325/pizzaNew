using System.Windows;

namespace pizza
{
    /// <summary>
    /// Interaction logic for MyProfile.xaml
    /// </summary>
    public partial class MyProfile : Window
    {
        public MyProfile()
        {
            InitializeComponent();
            LoadProfile();
        }

        private void LoadProfile()
        {
            if (!App.IsUserLoggedIn)
            {
                ((App)Application.Current).OpenWindow(new Login(), this);
                return;
            }

            FullNameText.Text = App.CurrentUserFullName;
            EmailText.Text = App.CurrentUserEmail;
            PhoneText.Text = App.CurrentUserPhone;
            AddressText.Text = App.CurrentUserAddress;
            BirthdayText.Text = App.CurrentUserBday;
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            App.LogoutCurrentUser();
            ((App)Application.Current).OpenWindow(new MainWindow(), this);
        }
    }
}
