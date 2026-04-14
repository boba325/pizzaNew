using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace pizza
{
    public partial class MenuPizza : Window
    {
        private readonly Dictionary<string, string> _buttonTextToToppingKey = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["פפרוני"] = "Pepperoni",
            ["אנשובי"] = "Anchovi",
            ["טונה"] = "Tuna",
            ["זיתים"] = "Olives",
            ["בצל"] = "Onion",
            ["פלפל ירוק"] = "GreenPepper",
            ["אננס"] = "Pineapple",
            ["קבנוס"] = "Kabanos"
        };

        private readonly HashSet<int> _selectedSlices = new HashSet<int>();
        private readonly Dictionary<int, HashSet<string>> _sliceToppings = new Dictionary<int, HashSet<string>>();
        private readonly Dictionary<string, string> _toppingImagePaths = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Pepperoni"] = "pack://application:,,,/images/Toppings/Pepproni.png",
            ["Anchovi"] = "pack://application:,,,/images/Toppings/Anchovi.png",
            ["Tuna"] = "pack://application:,,,/images/Toppings/Tuna.png",
            ["Olives"] = "pack://application:,,,/images/Toppings/Olive.png",
            ["Onion"] = "pack://application:,,,/images/Toppings/Onion.png",
            ["GreenPepper"] = "pack://application:,,,/images/Toppings/Pepper.png",
            ["Pineapple"] = "pack://application:,,,/images/Toppings/Pineapple.png",
            ["Kabanos"] = "pack://application:,,,/images/Toppings/Kabanos.png"
        };

        private static readonly Brush HoverBrush = new SolidColorBrush(Color.FromArgb(160, 255, 165, 0));
        private static readonly DropShadowEffect GlowEffect = new DropShadowEffect
        {
            Color = Colors.OrangeRed,
            BlurRadius = 28,
            ShadowDepth = 0,
            Opacity = 0.95
        };

        private const int SliceCount = 8;

        private string _activeTopping = string.Empty;
        private Button _activeToppingButton;
        private Point _pizzaCenter;
        private double _pizzaRadius;

        public MenuPizza()
        {
            InitializeComponent();

            PizzaCanvas.Loaded += (s, e) => RedrawPizzaCanvas();
            PizzaCanvas.SizeChanged += (s, e) => RedrawPizzaCanvas();
        }

        private void RedrawPizzaCanvas()
        {
            double width = PizzaCanvas.ActualWidth;
            double height = PizzaCanvas.ActualHeight;
            if (width <= 0 || height <= 0)
            {
                return;
            }

            PizzaCanvas.Children.Clear();
            _pizzaCenter = new Point(width / 2.0, height / 2.0);
            _pizzaRadius = Math.Min(width, height) / 2.0;

            DrawToppings();
            DrawSlices();
        }

        private void DrawSlices()
        {
            double step = 360.0 / SliceCount;
            double startAngleOffset = -90.0;

            for (int i = 0; i < SliceCount; i++)
            {
                double angleStart = startAngleOffset + i * step;
                double angleEnd = angleStart + step;

                Point p1 = PointOnCircle(_pizzaCenter, _pizzaRadius, angleStart);
                Point p2 = PointOnCircle(_pizzaCenter, _pizzaRadius, angleEnd);

                var figure = new PathFigure
                {
                    StartPoint = _pizzaCenter,
                    IsClosed = true,
                    IsFilled = true
                };
                figure.Segments.Add(new LineSegment(p1, true));
                figure.Segments.Add(new ArcSegment
                {
                    Point = p2,
                    Size = new Size(_pizzaRadius, _pizzaRadius),
                    RotationAngle = 0,
                    IsLargeArc = step > 180,
                    SweepDirection = SweepDirection.Clockwise,
                    IsStroked = false
                });

                var path = new Path
                {
                    Data = new PathGeometry(new[] { figure }),
                    Fill = _selectedSlices.Contains(i) ? HoverBrush : Brushes.Transparent,
                    Stroke = Brushes.Transparent,
                    Effect = _selectedSlices.Contains(i) ? GlowEffect : null,
                    Tag = i,
                    Cursor = Cursors.Hand
                };

                path.MouseEnter += Slice_MouseEnter;
                path.MouseLeave += Slice_MouseLeave;
                path.MouseLeftButtonUp += Slice_MouseLeftButtonUp;

                Canvas.SetLeft(path, 0);
                Canvas.SetTop(path, 0);
                Panel.SetZIndex(path, 10);
                PizzaCanvas.Children.Add(path);
            }
        }

        private void DrawToppings()
        {
            foreach (KeyValuePair<int, HashSet<string>> sliceEntry in _sliceToppings)
            {
                int toppingIndex = 0;
                foreach (string toppingName in sliceEntry.Value)
                {
                    Point location = GetToppingLocation(sliceEntry.Key, toppingIndex++);
                    FrameworkElement toppingVisual = CreateToppingVisual(toppingName);
                    toppingVisual.IsHitTestVisible = false;

                    Canvas.SetLeft(toppingVisual, location.X - (toppingVisual.Width / 2.0));
                    Canvas.SetTop(toppingVisual, location.Y - (toppingVisual.Height / 2.0));
                    Panel.SetZIndex(toppingVisual, 1);
                    PizzaCanvas.Children.Add(toppingVisual);
                }
            }
        }

        private FrameworkElement CreateToppingVisual(string toppingName)
        {
            if (_toppingImagePaths.TryGetValue(toppingName, out string imagePath))
            {
                return new Image
                {
                    Width = 44,
                    Height = 44,
                    Stretch = Stretch.UniformToFill,
                    Source = new BitmapImage(new Uri(imagePath, UriKind.Absolute))
                };
            }

            return new Border
            {
                Width = 58,
                Height = 30,
                Background = new SolidColorBrush(Color.FromArgb(220, 255, 248, 220)),
                BorderBrush = Brushes.DarkRed,
                BorderThickness = new Thickness(1.2),
                CornerRadius = new CornerRadius(12),
                Child = new TextBlock
                {
                    Text = toppingName,
                    FontSize = 10,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.DarkRed,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(4, 2, 4, 2)
                }
            };
        }

        private Point GetToppingLocation(int sliceIndex, int toppingIndex)
        {
            double step = 360.0 / SliceCount;
            double midAngle = -90.0 + (sliceIndex * step) + (step / 2.0);
            Point basePoint = PointOnCircle(_pizzaCenter, _pizzaRadius * 0.62, midAngle);

            Point[] offsets =
            {
                new Point(0, 0),
                new Point(28, -12),
                new Point(-28, -10),
                new Point(18, 24),
                new Point(-22, 22)
            };

            Point offset = offsets[toppingIndex % offsets.Length];
            return new Point(basePoint.X + offset.X, basePoint.Y + offset.Y);
        }

        private static Point PointOnCircle(Point center, double radius, double angleDegrees)
        {
            double angleRad = angleDegrees * Math.PI / 180.0;
            double x = center.X + radius * Math.Cos(angleRad);
            double y = center.Y + radius * Math.Sin(angleRad);
            return new Point(x, y);
        }

        private void Slice_MouseEnter(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_activeTopping))
            {
                return;
            }

            if (sender is Path path)
            {
                int idx = Convert.ToInt32(path.Tag);
                if (!_selectedSlices.Contains(idx))
                {
                    path.Fill = HoverBrush;
                    path.Effect = GlowEffect;
                }
            }
        }

        private void Slice_MouseLeave(object sender, MouseEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_activeTopping))
            {
                return;
            }

            if (sender is Path path)
            {
                int idx = Convert.ToInt32(path.Tag);
                if (!_selectedSlices.Contains(idx))
                {
                    path.Fill = Brushes.Transparent;
                    path.Effect = null;
                }
            }
        }

        private void Slice_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_activeTopping))
            {
                return;
            }

            if (!(sender is Path path))
            {
                return;
            }

            int idx = Convert.ToInt32(path.Tag);
            if (_selectedSlices.Contains(idx))
            {
                _selectedSlices.Remove(idx);
                path.Fill = Brushes.Transparent;
                path.Effect = null;
            }
            else
            {
                _selectedSlices.Add(idx);
                path.Fill = HoverBrush;
                path.Effect = GlowEffect;
            }

            AddToppingToSelectedSlices(_activeTopping);
        }

        private void ToppingButton_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button button))
            {
                return;
            }

            string buttonText = button.Content?.ToString() ?? string.Empty;
            if (!_buttonTextToToppingKey.TryGetValue(buttonText, out string toppingName))
            {
                return;
            }

            bool changedTopping = !string.Equals(_activeTopping, toppingName, StringComparison.OrdinalIgnoreCase);
            if (changedTopping)
            {
                ClearSelectedSlices();
            }

            SetActiveTopping(button, toppingName);
        }

        private void AddToppingToSelectedSlices(string toppingName)
        {
            bool addedAnything = false;

            foreach (int sliceIndex in _selectedSlices)
            {
                if (!_sliceToppings.TryGetValue(sliceIndex, out HashSet<string> toppings))
                {
                    toppings = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                    _sliceToppings[sliceIndex] = toppings;
                }

                if (toppings.Add(toppingName))
                {
                    addedAnything = true;
                }
            }

            if (addedAnything)
            {
                RedrawPizzaCanvas();
            }
        }

        private void SetActiveTopping(Button button, string toppingName)
        {
            if (_activeToppingButton != null)
            {
                _activeToppingButton.ClearValue(Button.BackgroundProperty);
                _activeToppingButton.Effect = null;
            }

            _activeTopping = toppingName;
            _activeToppingButton = button;
            _activeToppingButton.Background = HoverBrush;
            _activeToppingButton.Effect = GlowEffect;
        }

        private void ClearSelectedSlices()
        {
            _selectedSlices.Clear();
            RedrawPizzaCanvas();
        }

        private void ClearToppingsButton_Click(object sender, RoutedEventArgs e)
        {
            _sliceToppings.Clear();
            _selectedSlices.Clear();
            _activeTopping = string.Empty;

            if (_activeToppingButton != null)
            {
                _activeToppingButton.ClearValue(Button.BackgroundProperty);
                _activeToppingButton.Effect = null;
                _activeToppingButton = null;
            }

            RedrawPizzaCanvas();
        }

        private void AddToOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!App.IsUserLoggedIn)
            {
                MessageBox.Show("אנא כנס לפני שאתה מזמין", "Login Required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var slices = BuildPizzaSlices();
            if (slices.Count != SliceCount)
            {
                MessageBox.Show("לפיצה חייבים להיות 8 משולשים", "Pizza Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ServiceReference1.Service1Client service = new ServiceReference1.Service1Client();
            //var orderBll = new OrderBLL();
            try
            {
                int orderId = service.AddPizzaToCurrentOrder(App.CurrentUserId, App.CurrentUserAddress, slices.ToArray());
                App.SetCurrentOrderId(orderId);
                MessageBox.Show("הפיצה הוספה להזמנה", "Order Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (System.InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Order Locked", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private List<pizza.ServiceReference1.PizzaSlice> BuildPizzaSlices()
        {
            var slices = new List<pizza.ServiceReference1.PizzaSlice>();
            for (int i = 0; i < SliceCount; i++)
            {
                _sliceToppings.TryGetValue(i, out HashSet<string> toppings);
                toppings = toppings ?? new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                slices.Add(new pizza.ServiceReference1.PizzaSlice
                {
                    SliceNumber = i + 1,
                    Pepperoni = toppings.Contains("Pepperoni"),
                    Anchovi = toppings.Contains("Anchovi"),
                    Tuna = toppings.Contains("Tuna"),
                    Olives = toppings.Contains("Olives"),
                    Onion = toppings.Contains("Onion"),
                    GreenPepper = toppings.Contains("GreenPepper"),
                    Pineapple = toppings.Contains("Pineapple"),
                    Kabanos = toppings.Contains("Kabanos"),
                    Amount = 1
                });
            }

            return slices;
        }
    }
}
