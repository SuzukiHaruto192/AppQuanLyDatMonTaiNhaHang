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
using QuanLyBan.HoaDon;

namespace QuanLyBan
{
    public partial class InvoiceWindow : Window
    {

        public InvoiceWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void ConfirmPaymentButton_Click(object sender, RoutedEventArgs e)
        {
           // string table = App
            DatabaseHelper db = new DatabaseHelper();
            db.KhachDi(((App)Application.Current).current_maban, "Trống");
            db.UpdateHoaDon(((App)Application.Current).current_mahd, "Đã Thanh Toán");
            this.DialogResult = true;

        }
    }
}
