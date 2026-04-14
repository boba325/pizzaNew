using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace pizza
{
    public partial class AdminOrders : Window
    {
        public AdminOrders()
        {
            InitializeComponent();
            LoadOrders();
        }

        private void LoadOrders()
        {
            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var orderBll = new OrderBLL();
            OrdersGrid.ItemsSource = service.GetOrdersList();
        }

        private void OrdersGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
            {
                return;
            }

            if (!(e.Row.Item is pizza.ServiceReference1.Order order))
            {
                return;
            }

            //var orderBll = new OrderBLL();
            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            service.UpdateOrder(order);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(OrdersGrid.SelectedItem is pizza.ServiceReference1.Order order))
            {
                MessageBox.Show("בחר הזמנה למחיקה.");
                return;
            }

            var result = MessageBox.Show($"למחוק את הזמנה {order.Id}?", "Delete Order", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var orderBll = new OrderBLL();
            service.DeleteOrder(order);
            LoadOrders();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var popup = new AddOrderPopup
            {
                Owner = this
            };

            bool? result = popup.ShowDialog();
            if (result != true || popup.CreatedOrder == null)
            {
                return;
            }

            //var orderBll = new OrderBLL();
            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            service.AddOrder(popup.CreatedOrder);
            LoadOrders();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            OrdersGrid.SelectedItem = null;
            OrdersGrid.UnselectAll();
        }

        private void DecreaseLevel_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.DataContext is pizza.ServiceReference1.Order order))
            {
                return;
            }

            if (order.OrderLevel <= 0)
            {
                return;
            }

            order.OrderLevel -= 1;
            //var orderBll = new OrderBLL();
            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            service.UpdateOrder(order);
            OrdersGrid.Items.Refresh();
        }

        private void IncreaseLevel_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.DataContext is pizza.ServiceReference1.Order order))
            {
                return;
            }

            if (order.OrderLevel >= 3)
            {
                return;
            }

            order.OrderLevel += 1;
            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            service.UpdateOrder(order);
            OrdersGrid.Items.Refresh();
        }

        private void ViewProducts_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.DataContext is pizza.ServiceReference1.Order order))
            {
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var orderBll = new OrderBLL();
            var items = service.GetOrderItemsSummary(order.Id);
            if (items == null || items.Length == 0)
            {
                MessageBox.Show("No products in this order.");
                return;
            }

            string message = string.Join("\n", items.Select(item =>
                string.IsNullOrWhiteSpace(item.Details)
                    ? $"{item.ProductName} x{item.Quantity}"
                    : $"{item.ProductName} x{item.Quantity} | {item.Details}"));

            MessageBox.Show(message, $"Order {order.Id} Products", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ToggleApproval_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || !(button.DataContext is pizza.ServiceReference1.Order order))
            {
                return;
            }

            if (order.IsApproved)
            {
                order.IsApproved = false;
                order.OrderLevel = 0;
            }
            else
            {
                order.IsApproved = true;
                order.OrderLevel = 1;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            service.UpdateOrder(order);
            LoadOrders();
        }
    }
}
