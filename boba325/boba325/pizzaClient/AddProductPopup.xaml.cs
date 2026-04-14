using System.Globalization;
using System.Windows;

namespace pizza
{
    public partial class AddProductPopup : Window
    {
        public AddProductPopup()
        {
            InitializeComponent();
            CategoryComboBox.ItemsSource = new[] { "Pizza", "Drinks", "Desserts" };
            CategoryComboBox.SelectedIndex = 0;
        }

        ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
        public pizza.ServiceReference1.Product CreatedProduct { get; private set; }

        private void Done_Click(object sender, RoutedEventArgs e)
        {
            string name = (NameBox.Text ?? string.Empty).Trim();
            string category = CategoryComboBox.SelectedItem as string;

            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show("הכנס שם מוצר.");
                return;
            }

            if (string.IsNullOrWhiteSpace(category))
            {
                MessageBox.Show("בחר קטגוריה.");
                return;
            }

            if (!decimal.TryParse(PriceBox.Text, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal price) &&
                !decimal.TryParse(PriceBox.Text, NumberStyles.Number, CultureInfo.CurrentCulture, out price))
            {
                MessageBox.Show("הכנס מחיר תקין.");
                return;
            }

            CreatedProduct = new pizza.ServiceReference1.Product
            {
                Name = name,
                Category = category,
                Price = price,
                IsCoupon = false,
                Picture = string.Empty
            };

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
