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
using doanlttq.Qrcode;

namespace doanlttq.Qrcode
{
    /// <summary>
    /// Interaction logic for HienQrThanhToan.xaml
    /// </summary>
    public partial class HienQrThanhToan : Window
    {
        public HienQrThanhToan(BitmapImage QRImage)
        {
            InitializeComponent();
            QR.Source = QRImage;
        }
        private async Task KiemTraGiaoDich()
        {
            // Giả lập thời gian xử lý thanh toán (chờ 3 giây)
            await Task.Delay(4000);

            // ✅ Sau khi "thanh toán xong"
            StatusText.Text = "✅ Thanh toán thành công!";
            StatusText.Foreground = System.Windows.Media.Brushes.Green;

            // Đóng cửa sổ sau 2 giây
            await Task.Delay(10000);
            this.Close();
        }
        private void XN_Thanh_Toan(object sender, EventArgs e)
        {
            login lg= new login();
            lg.Show();
            this.Close();
        }
    }
}
