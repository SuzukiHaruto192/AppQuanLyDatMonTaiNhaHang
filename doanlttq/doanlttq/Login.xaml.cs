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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace doanlttq
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DoubleAnimation grow = new DoubleAnimation
            {
                To = 1.2,
                Duration = TimeSpan.FromSeconds(0.1),
                        AutoReverse = true,
            };

            LoginButtonTransform.BeginAnimation(ScaleTransform.ScaleXProperty, grow);
            LoginButtonTransform.BeginAnimation(ScaleTransform.ScaleYProperty, grow);
        }

        private void Mouse_Enter_Button(object sender, MouseEventArgs e)
        {
            DoubleAnimation grow = new DoubleAnimation
            {
                To = 1.1, 
                Duration = TimeSpan.FromSeconds(0.1)
            };

            LoginButtonTransform.BeginAnimation(ScaleTransform.ScaleXProperty, grow);
            LoginButtonTransform.BeginAnimation(ScaleTransform.ScaleYProperty, grow);
        }

        private void Mouse_Leave_Button(object sender, MouseEventArgs e)
        {
            DoubleAnimation grow = new DoubleAnimation
            {
                To = 1.0,
                Duration = TimeSpan.FromSeconds(0.1)
            };

            LoginButtonTransform.BeginAnimation(ScaleTransform.ScaleXProperty, grow);
            LoginButtonTransform.BeginAnimation(ScaleTransform.ScaleYProperty, grow);
        }
        void Forget_Password(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("hello");
        }
        void Hidden_Password(object sender, RoutedEventArgs e)
        {
            if (Border_PasswordHidden.Visibility == Visibility.Visible) {
                User_Password.Text = User_Password_Hidden.Password;
                Border_PasswordVisible.Visibility = Visibility.Visible; 
                Border_PasswordHidden.Visibility = Visibility.Hidden;
            }
            else
            {
                User_Password_Hidden.Password = User_Password.Text;
                Border_PasswordHidden.Visibility= Visibility.Visible;
                Border_PasswordVisible.Visibility= Visibility.Hidden;
            }
        }
    }
}
