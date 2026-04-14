using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace pizza
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
            UpdateSecondaryButton();
        }

        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameBox.Text;
            string password = PasswordBox.Password;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("אנא הזן שם משתמש וסיסמה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            //  var userBll = new UserBLL();
            //  var users = userBll.GetUsersList();

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            var users = service.GetUsersList().ToList();

            if (users == null || users.Count == 0)
            {
                MessageBox.Show("לא ניתן לגשת לרשימת המשתמשים כעת.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }


            //var user = users.FirstOrDefault(u => string.Equals(u.Email, username, StringComparison.OrdinalIgnoreCase));
            var user = service.GetUserByEmail(username);

            if (user == null)
            {
                MessageBox.Show("אופס! שם משתמש שגוי, נסה שוב", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (user.Password == password)
            {
                App.SetCurrentUser(user);
                MessageBox.Show("נכנסת בהצלחה!", "ברוך שובך", MessageBoxButton.OK, MessageBoxImage.Information);
                ((App)Application.Current).OpenWindow(new MainWindow(), this);
            }
            else if (user.Password != password && user.Email == username)
            {
                MessageBox.Show("אופס! סיסמה לא נכונה, נסה שוב", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("אופס! משהו השתבש", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Signup_Click(object sender, RoutedEventArgs e)
        {
            if (App.IsUserLoggedIn)
            {
                ((App)Application.Current).OpenWindow(new MyProfile(), this);
                return;
            }

            SignupWindow signupWindow = new SignupWindow
            {
                Owner = this,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            signupWindow.ShowDialog();
            UpdateSecondaryButton();
        }

        private void UpdateSecondaryButton()
        {
            SignupButton.Content = App.IsUserLoggedIn ? "הפרופיל שלי" : "הרשמה";
        }
    }
}
