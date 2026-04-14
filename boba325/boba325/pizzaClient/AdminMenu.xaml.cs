using System.Windows;
using System.Windows.Controls;

namespace pizza
{
    public partial class AdminMenu : Window
    {
        public AdminMenu()
        {
            InitializeComponent();
            LoadProducts();
        }

        private void LoadProducts()
        {
            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var productBll = new ProductBLL();
            ProductsGrid.ItemsSource = service.GetProductsList();
        }

        private void ProductsGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction != DataGridEditAction.Commit)
            {
                return;
            }

            if (!(e.Row.Item is pizza.ServiceReference1.Product product))
            {
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var productBll = new ProductBLL();
            service.UpdateProduct(product);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(ProductsGrid.SelectedItem is pizza.ServiceReference1.Product product))
            {
                MessageBox.Show("בחר מוצר למחיקה.");
                return;
            }

            var result = MessageBox.Show($"למחוק את {product.Name}?", "Delete Product", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result != MessageBoxResult.Yes)
            {
                return;
            }

            //var productBll = new ProductBLL();
            //productBll.DeleteProduct(product.Id);
            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            service.DeleteProduct(product);
            LoadProducts();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            var popup = new AddProductPopup
            {
                Owner = this
            };

            bool? result = popup.ShowDialog();
            if (result != true || popup.CreatedProduct == null)
            {
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var productBll = new ProductBLL();
            //productBll.AddProduct(popup.CreatedProduct);
            service.AddProduct(popup.CreatedProduct);
            LoadProducts();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ProductsGrid.SelectedItem = null;
            ProductsGrid.UnselectAll();
        }
    }
}
