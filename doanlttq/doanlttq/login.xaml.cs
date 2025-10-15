using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace doanlttq
{
    /// <summary>
    /// Interaction logic for login.xaml
    /// </summary>
    public partial class login : Window
    {
        public login()
        {
            InitializeComponent();
        }

        private void Mouse_Enter(object sender, MouseEventArgs e)
        {
            (sender as Button).Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xEE, 0x6E, 0x18));
            (sender as Button).BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xEE, 0x6E, 0x18));
        }

        private void Mouse_Leave(object sender, MouseEventArgs e)
        {
            (sender as Button).Foreground = new SolidColorBrush(Colors.Gray);
            (sender as Button).BorderBrush = new SolidColorBrush(Colors.Gray);
        }

        private void Click_Button(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
