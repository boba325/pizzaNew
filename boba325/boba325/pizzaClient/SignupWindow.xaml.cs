using System;
using System.Text.RegularExpressions;
using System.Windows;

namespace pizza
{
    public partial class SignupWindow : Window
    {
        private static readonly char[] SpecialChars = new[] { '!', '@', '#', '$', '%', '^', '&', '*', '(', ')', '<', '>' };

        public SignupWindow()
        {
            InitializeComponent();
        }

        private void Signup_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(FirstNameBox.Text))
            {
                MessageBox.Show("אנא הזן שם פרטי.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(LastNameBox.Text))
            {
                MessageBox.Show("אנא הזן שם משפחה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string email = (EmailBox.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(email))
            {
                MessageBox.Show("אנא הזן אימייל.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!IsValidEmail(email))
            {
                MessageBox.Show("אנא הזן כתובת אימייל תקינה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string password = PasswordBox.Password ?? string.Empty;
            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("אנא הזן סיסמה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string confirmPassword = ConfirmPasswordBox.Password ?? string.Empty;
            if (string.IsNullOrWhiteSpace(confirmPassword))
            {
                MessageBox.Show("אנא אשר את הסיסמה.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!string.Equals(password, confirmPassword, StringComparison.Ordinal))
            {
                MessageBox.Show("הסיסמאות אינן זהות. אנא ודא שסיסמה ואישור הסיסמה זהים.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("הסיסמה חייבת להכיל לפחות 6 תווים.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!ContainsSpecialChar(password))
            {
                MessageBox.Show("הסיסמה חייבת להכיל לפחות תו מיוחד אחד מתוך: !,@,#,$,%,^,&,*,(,),<,>", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            string phone = (PhoneBox.Text ?? string.Empty).Trim();
            if (string.IsNullOrWhiteSpace(phone))
            {
                MessageBox.Show("אנא הזן מספר טלפון.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!Regex.IsMatch(phone, @"^\d{10}$"))
            {
                MessageBox.Show("אנא הזן מספר טלפון תקין — 10 ספרות בלבד (רק מספרים).", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(AddressBox.Text))
            {
                MessageBox.Show("אנא הזן כתובת.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(BirthdayBox.Text))
            {
                MessageBox.Show("אנא הזן תאריך לידה בפורמט YYYY-MM-DD.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!DateTime.TryParse(BirthdayBox.Text, out DateTime birthday))
            {
                MessageBox.Show("תאריך לידה לא תקין. אנא הזן בפורמט YYYY-MM-DD.", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            pizza.ServiceReference1.User newUser = new pizza.ServiceReference1.User
            {
                FName = FirstNameBox.Text,
                LName = LastNameBox.Text,
                Email = email,
                Password = password,
                Phone = phone,
                Address = AddressBox.Text,
                Bday = BirthdayBox.Text,
                IsAdmin = false
            };

            try
            {
                //UserBLL userBll = new UserBLL();
                service.AddUser(newUser);
                MessageBox.Show("הרשמה בוצעה בהצלחה!", "הצלחה", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"שגיאה: {ex.Message}", "שגיאה", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            const string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
        }

        private static bool ContainsSpecialChar(string s)
        {
            foreach (char c in s)
            {
                foreach (char sc in SpecialChars)
                {
                    if (c == sc) return true;
                }
            }
            return false;
        }
    }
}
