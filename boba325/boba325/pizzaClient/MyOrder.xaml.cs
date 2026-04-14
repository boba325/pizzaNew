using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;


namespace pizza
{
    public partial class MyOrder : Window
    {
        public MyOrder()
        {
            InitializeComponent();
            LoadCurrentOrder();
        }

        private void LoadCurrentOrder()
        {
            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            var rows = new List<ServiceReference1.OrderItemSummary>();
            int orderLevel = 0;

            if (App.IsUserLoggedIn)
            {
                //var orderBll = new OrderBLL();
                pizza.ServiceReference1.Order openOrder = service.GetOpenOrderForUser(App.CurrentUserId);
                if (openOrder != null)
                {
                    App.SetCurrentOrderId(openOrder.Id);
                    orderLevel = openOrder.OrderLevel;
                    rows = service.GetOrderItemsSummary(openOrder.Id).ToList();
                }
                else
                {
                    App.SetCurrentOrderId(0);
                }
            }

            decimal total = rows.Sum(x => x.LineTotal);
            rows.Add(new pizza.ServiceReference1.OrderItemSummary
            {
                ProductName = "סהכ",
                Category = string.Empty,
                UnitPrice = 0m,
                Quantity = 0,
                SaleText = string.Empty,
                LineTotal = total,
                IsSummaryRow = true
            });

            OrderItemsGrid.ItemsSource = rows;

            if (orderLevel >= 2)
            {
                ShowOrderStatus(orderLevel);
                return;
            }

            ShowOrderTable();
        }

        private void IncreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || App.CurrentOrderId <= 0)
            {
                return;
            }

            if (!(button.DataContext is pizza.ServiceReference1.OrderItemSummary item) || item.IsSummaryRow)
            {
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var orderBll = new OrderBLL();
            if (item.IsPizza)
            {
                service.IncreasePizzaQuantity(item.PizzaId);
            }
            else
            {
                service.IncreaseProductQuantity(App.CurrentOrderId, item.ProductId);
            }

            LoadCurrentOrder();
        }

        private void DecreaseAmount_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button) || App.CurrentOrderId <= 0)
            {
                return;
            }

            if (!(button.DataContext is pizza.ServiceReference1.OrderItemSummary item) || item.IsSummaryRow)
            {
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var orderBll = new OrderBLL();
            if (item.IsPizza)
            {
                service.DecreasePizzaQuantity(item.PizzaId);
            }
            else
            {
                service.DecreaseProductQuantity(App.CurrentOrderId, item.ProductId);
            }

            LoadCurrentOrder();
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!App.IsUserLoggedIn)
            {
                MessageBox.Show("אנא כנס לפני שאתה מזמין");
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            // var orderBll = new OrderBLL();
            pizza.ServiceReference1.Order openOrder = service.GetOpenOrderForUser(App.CurrentUserId);
            if (openOrder == null)
            {
                MessageBox.Show("ההזמנה ריקה");
                return;
            }

            var orderItems = service.GetOrderItemsSummary(openOrder.Id).ToList();
            if (orderItems.Count == 0)
            {
                MessageBox.Show("אנא הכנס מוצרים להזמנה");
                return;
            }

            //var creditCardBll = new CreditCardBLL();
            if (!service.HasSavedCard(App.CurrentUserId))
            {
                var popup = new CreditCardPopup
                {
                    Owner = this
                };

                bool? popupResult = popup.ShowDialog();
                if (popupResult != true || popup.EnteredCard == null)
                {
                    return;
                }

                //popup.EnteredCard.Type = service.DetectCardType(popup.EnteredCard.Number);
                if (popup.ShouldSaveCard)
                {
                    service.SaveCard(popup.EnteredCard);
                }
            }

            service.UpdateOrderLevel(openOrder.Id, 2);
            LoadCurrentOrder();
        }

        private void ShowOrderTable()
        {
            OrderTableSection.Visibility = Visibility.Visible;
            OrderStatusSection.Visibility = Visibility.Collapsed;
        }

        private void ShowOrderStatus(int orderLevel)
        {
            OrderTableSection.Visibility = Visibility.Collapsed;
            OrderStatusSection.Visibility = Visibility.Visible;

            Level2Triangle.Visibility = Visibility.Collapsed;
            Level3Triangle.Visibility = Visibility.Collapsed;
            Level4Triangle.Visibility = Visibility.Collapsed;

            if (orderLevel == 2)
            {
                Level2Triangle.Visibility = Visibility.Visible;
                StatusTitleText.Text = "ההזמנה אושרה";
                StatusDescriptionText.Text = "קיבלנו את הזמנתך והיא בהכנה כרגע";
                return;
            }

            if (orderLevel == 3)
            {
                Level3Triangle.Visibility = Visibility.Visible;
                StatusTitleText.Text = "הזמנה באריזה";
                StatusDescriptionText.Text = "הזמנתך באריזה, רק עוד קצת!";
                return;
            }

            Level4Triangle.Visibility = Visibility.Visible;
            StatusTitleText.Text = "הזמנה בדרך";
            StatusDescriptionText.Text = "השליח אסף את ההזמנה והיא בדרכה אלייך!";
        }

        private void CancelOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!App.IsUserLoggedIn)
            {
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var orderBll = new OrderBLL();
            pizza.ServiceReference1.Order openOrder = service.GetOpenOrderForUser(App.CurrentUserId);
            if (openOrder == null)
            {
                return;
            }

            service.UpdateOrderLevel(openOrder.Id, 1);
            LoadCurrentOrder();
        }
    }
}
