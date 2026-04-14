using System;
using System.Windows;

namespace pizza
{
    public partial class AddOrderPopup : Window
    {
        public AddOrderPopup()
        {
            InitializeComponent();
            DateBox.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            OrderLevelBox.Text = "0";
        }

        // public Order CreatedOrder { get; private set; }
        ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
        public ServiceReference1.Order CreatedOrder { get; private set; }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(UserIdBox.Text, out int userId))
            {
                MessageBox.Show("הכנס UserID תקין.");
                return;
            }

            if (!int.TryParse(OrderLevelBox.Text, out int orderLevel))
            {
                MessageBox.Show("הכנס Order Level תקין.");
                return;
            }

            if (orderLevel < 0 || orderLevel > 3)
            {
                MessageBox.Show("סטטוס הזמנה חייב להיות בין 0 ל3");
                return;
            }

            CreatedOrder = new ServiceReference1.Order
            {
                UserId = userId,
                Address = (AddressBox.Text ?? string.Empty).Trim(),
                Date = (DateBox.Text ?? string.Empty).Trim(),
                IsApproved = IsApprovedCheckBox.IsChecked == true,
                OrderLevel = orderLevel
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
