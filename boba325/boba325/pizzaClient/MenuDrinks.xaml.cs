using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace pizza
{
    public partial class MenuDrinks : Window
    {
        private readonly Dictionary<string, (string Name, decimal Price, string Picture)> _drinkProducts =
            new Dictionary<string, (string Name, decimal Price, string Picture)>
            {
                ["coca"] = ("Coca Cola", 14.00m, "images/DrinkMenu/coca.png"),
                ["cocaZero"] = ("Coca Cola Zero", 14.00m, "images/DrinkMenu/cocaZero.png"),
                ["sprite"] = ("Sprite", 14.00m, "images/DrinkMenu/sprite.png"),
                ["water"] = ("Water", 10.00m, "images/DrinkMenu/water.png")
            };

        public MenuDrinks()
        {
            InitializeComponent();
        }

        private void AddDrinkToOrder_Click(object sender, RoutedEventArgs e)
        {
            if (!App.IsUserLoggedIn)
            {
                MessageBox.Show("אנא כנס לפני שאתה מזמין", "Login Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (!(sender is Button button) || button.Tag == null)
            {
                return;
            }

            string key = button.Tag.ToString();
            if (!_drinkProducts.TryGetValue(key, out var product))
            {
                return;
            }

            //var orderBll = new OrderBLL();
            try
            {
                ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
                int orderId = service.AddProductToCurrentOrder(App.CurrentUserId, App.CurrentUserAddress, product.Name, "Drinks", product.Price, product.Picture);
                App.SetCurrentOrderId(orderId);
                MessageBox.Show($"{product.Name} הוסף להזמנה", "Order Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Order Locked", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
