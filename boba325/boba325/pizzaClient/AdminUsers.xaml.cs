using System.Windows;
using System.Windows.Controls;

namespace pizza
{
    public partial class AdminUsers : Window
    {
        public AdminUsers()
        {
            InitializeComponent();
            LoadUsers();
        }

        private void LoadUsers()
        {
            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var userBll = new UserBLL();
            UsersGrid.ItemsSource = service.GetUsersList();
        }

        private void UsersGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
            {
                return;
            }

            if (!(e.Row.Item is pizza.ServiceReference1.User user))
            {
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            service.UpdateUser(user);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(UsersGrid.SelectedItem is pizza.ServiceReference1.User user))
            {
                MessageBox.Show("בחר משתמש למחיקה.");
                return;
            }

            var result = MessageBox.Show($"למחוק את {user.FName} {user.LName}?", "Delete User", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            //var userBll = new UserBLL();
            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            service.DeleteUser(user);
            LoadUsers();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var popup = new AddUserPopup
            {
                Owner = this
            };

            bool? result = popup.ShowDialog();
            if (result != true || popup.CreatedUser == null)
            {
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var userBll = new UserBLL();
            service.AddUser(popup.CreatedUser);
            LoadUsers();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            UsersGrid.SelectedItem = null;
            UsersGrid.UnselectAll();
        }
    }
}
