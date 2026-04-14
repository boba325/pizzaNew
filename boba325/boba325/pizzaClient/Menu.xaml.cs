using System.Windows;

namespace pizza
{
    public partial class Menu : Window
    {
        public Menu()
        {
            InitializeComponent();
        }

        private void OpenPizza_Click(object sender, RoutedEventArgs e)
        {
            OpenSameState(new MenuPizza());
        }

        private void OpenDrinks_Click(object sender, RoutedEventArgs e)
        {
            OpenSameState(new MenuDrinks());
        }

        private void OpenDesserts_Click(object sender, RoutedEventArgs e)
        {
            OpenSameState(new MenuDesserts());
        }

        private void OpenSameState(Window window)
        {
            window.WindowStartupLocation = WindowStartupLocation.Manual;

            if (WindowState == WindowState.Maximized)
            {
                window.WindowState = WindowState.Maximized;
            }
            else
            {
                window.Left = Left;
                window.Top = Top;
                window.Width = Width;
                window.Height = Height;
                window.WindowState = WindowState.Normal;
            }

            Application.Current.MainWindow = window;
            window.Show();
            Close();
        }
    }
}
