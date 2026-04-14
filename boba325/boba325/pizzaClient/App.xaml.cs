using System.Windows;

namespace pizza
{
    public partial class App : Application
    {
        public static int CurrentUserId { get; private set; }
        public static int CurrentOrderId { get; private set; }
        public static string CurrentUserFirstName { get; private set; } = string.Empty;
        public static string CurrentUserLastName { get; private set; } = string.Empty;
        public static string CurrentUserEmail { get; private set; } = string.Empty;
        public static string CurrentUserPhone { get; private set; } = string.Empty;
        public static string CurrentUserAddress { get; private set; } = string.Empty;
        public static string CurrentUserBday { get; private set; } = string.Empty;
        public static bool CurrentUserIsAdmin { get; private set; }
        public static bool IsUserLoggedIn => !string.IsNullOrEmpty(CurrentUserFirstName);
        public static string CurrentUserFullName => $"{CurrentUserFirstName} {CurrentUserLastName}".Trim();

        ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();

        public static void SetCurrentUser(pizza.ServiceReference1.User user)
        {
            if (user == null)
            {
                LogoutCurrentUser();
                return;
            }

            CurrentUserId = user.Id;
            CurrentOrderId = 0;
            CurrentUserFirstName = user.FName ?? string.Empty;
            CurrentUserLastName = user.LName ?? string.Empty;
            CurrentUserEmail = user.Email ?? string.Empty;
            CurrentUserPhone = user.Phone ?? string.Empty;
            CurrentUserAddress = user.Address ?? string.Empty;
            CurrentUserBday = user.Bday ?? string.Empty;
            CurrentUserIsAdmin = user.IsAdmin;
        }

        public static void SetCurrentOrderId(int orderId)
        {
            CurrentOrderId = orderId;
        }

        public static void LogoutCurrentUser()
        {
            CurrentUserId = 0;
            CurrentOrderId = 0;
            CurrentUserFirstName = string.Empty;
            CurrentUserLastName = string.Empty;
            CurrentUserEmail = string.Empty;
            CurrentUserPhone = string.Empty;
            CurrentUserAddress = string.Empty;
            CurrentUserBday = string.Empty;
            CurrentUserIsAdmin = false;
        }

        public void OpenWindow(Window newWindow, object sender)
        {
            DependencyObject src = sender as DependencyObject;
            Window caller = Window.GetWindow(src) ?? Application.Current.MainWindow;
            OpenWindow(newWindow, caller);
        }

        public void OpenWindow(Window newWindow, Window caller)
        {
            if (newWindow == null)
            {
                return;
            }

            newWindow.WindowStartupLocation = WindowStartupLocation.Manual;

            if (caller != null && caller.WindowState == WindowState.Maximized)
            {
                newWindow.WindowState = WindowState.Maximized;
            }
            else if (caller != null)
            {
                newWindow.Left = caller.Left;
                newWindow.Top = caller.Top;
                newWindow.Width = caller.Width;
                newWindow.Height = caller.Height;
                newWindow.WindowState = WindowState.Normal;
            }

            Application.Current.MainWindow = newWindow;
            newWindow.Show();

            if (caller != null)
            {
                caller.Close();
            }
        }

        public void GoHome_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new MainWindow(), sender);
        }

        public void OpenOrder_Click(object sender, RoutedEventArgs e)
        {
            if (IsUserLoggedIn)
            {
                OpenWindow(new MyOrder(), sender);
                return;
            }

            OpenWindow(new Login(), sender);
        }

        public void OpenLogin_Click(object sender, RoutedEventArgs e)
        {
            if (IsUserLoggedIn)
            {
                OpenWindow(new MyProfile(), sender);
                return;
            }

            OpenWindow(new Login(), sender);
        }

        public void OpenMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new Menu(), sender);
        }

        public void OpenAdminUsers_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new AdminUsers(), sender);
        }

        public void OpenAdminOrders_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new AdminOrders(), sender);
        }

        public void OpenAdminMenu_Click(object sender, RoutedEventArgs e)
        {
            OpenWindow(new AdminMenu(), sender);
        }

        public void Logout_Click(object sender, RoutedEventArgs e)
        {
            LogoutCurrentUser();
            OpenWindow(new MainWindow(), sender);
        }
    }
}
