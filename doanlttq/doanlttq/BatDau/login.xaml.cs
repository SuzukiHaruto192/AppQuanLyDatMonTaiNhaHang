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
using doanlttq;

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
            DatabaseHelper db= new DatabaseHelper();
            DateTime time = DateTime.Now;
            if (time.Hour >= 7 && time.Hour <= 12)
                Hello.Text = "GOOD MORNING!";
            else if (time.Hour > 12 && time.Hour <= 17)
                Hello.Text = "GOOD AFTERNOON!";
            else
                Hello.Text = "GOOD NIGHT!";

        }

        private void Click_Button(object sender, RoutedEventArgs e)
        {
            DatabaseHelper db = new DatabaseHelper();
            ((App)Application.Current).MABAN = db.TimBanTrong();
            ((App)Application.Current).MaHoaDon = "HD" + db.LayMaHoaDon();
            ((App)Application.Current).GioVao = DateTime.Now;
            ((App)Application.Current).ThemHDFirst = true;
            ((App)Application.Current).TongTien = 0;
            db.CoKhach(((App)Application.Current).MABAN);
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
