using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace pizza
{
    public partial class MenuDesserts : Window
    {
        private readonly Dictionary<string, (string Name, decimal Price, string Picture)> _dessertProducts =
            new Dictionary<string, (string Name, decimal Price, string Picture)>
            {
                ["cake"] = ("Cake", 22.00m, "images/DessertMenu/ccake.png"),
                ["iceCream"] = ("Ice Cream", 18.00m, "images/DessertMenu/iceCream.png"),
                ["souffle"] = ("Souffle", 24.00m, "images/DessertMenu/soufle.png")
            };

        public MenuDesserts()
        {
            InitializeComponent();
        }

        private void AddDessertToOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!App.IsUserLoggedIn)
            {
                MessageBox.Show("Please log in before adding items to an order.", "Login Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!(sender is Button button) || button.Tag == null)
            {
                return;
            }

            string key = button.Tag.ToString();
            if (!_dessertProducts.TryGetValue(key, out var product))
            {
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var orderBll = new OrderBLL();
            try
            {
                int orderId = service.AddProductToCurrentOrder(App.CurrentUserId, App.CurrentUserAddress, product.Name, "Desserts", product.Price, product.Picture);
                App.SetCurrentOrderId(orderId);
                MessageBox.Show($"{product.Name} was added to your order.", "Order Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Order Locked", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
