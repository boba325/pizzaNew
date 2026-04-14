using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace pizza
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {   
            InitializeComponent();

            if (!string.IsNullOrEmpty(App.CurrentUserFirstName))
            {
                UserNameText.Text = App.CurrentUserFirstName;
                UserNameText.Visibility = Visibility.Visible;
            }
            else
            {
                UserNameText.Visibility = Visibility.Collapsed;
            }

            if(App.CurrentUserIsAdmin)
            {
                Banner.ContentTemplate = (DataTemplate)FindResource("AdminHeaderTemplate");
            } 
        }
    }
}
