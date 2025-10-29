using doanlttq.MonAn;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace doanlttq
{
    /// <summary>
    /// Interaction logic for HoaDon.xaml
    /// </summary>
    public partial class HoaDon : Window
    {
        public ObservableCollection<Food> Foods { get; set; }
        decimal TongTien = 0;
        public HoaDon(ObservableCollection<Food> foods)
        {
            InitializeComponent();
            Foods = foods;
            this.DataContext = this;
            foreach (Food Food in foods)
            {
                TongTien += Food.GIA * Food.SoLuong;
            }
            Tong_Tien.Text = "Tổng Tiền : " + TongTien;
        }
        private void Quay_Lai(object sender, RoutedEventArgs e) 
        { 
            Gio_Hang gh= new Gio_Hang(Foods,Foods);
            gh.Show();
            this.Close();
        }
        private void Thanh_Toan(object sender, RoutedEventArgs e)
        {
            DatabaseHelper db = new DatabaseHelper();
            string MaKhachHang = Interaction.InputBox("", "Nhập Mã Khách Hàng", "");
            if (string.IsNullOrWhiteSpace(MaKhachHang) || db.TimMAKH(MaKhachHang) == false)
            {
                MessageBox.Show("Không có Mã Khách Hàng", "Thông Báo",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                MaKhachHang = "0";
            }
            if(MaKhachHang !="0")
            MessageBox.Show($"Mã Khách Hàng: {MaKhachHang} Quét QR để thanh toán");
            BitmapImage qrImage = Qr.TaoQr(
                    NganHang: "VCB",
                    STK: "9706101617",
                    ChuTaiKhoan: "LE DUY QUANG",
                    SoTien: TongTien,
                    GhiChu: "Thanh toan don hang #123"
                );

            ((App)Application.Current).GioRa = DateTime.Now;
            HienQrThanhToan qrtt = new HienQrThanhToan(qrImage);
            qrtt.ShowDialog();
            MessageBox.Show("Đơn hàng đã thanh toán thành công!", "Thông báo",
                            MessageBoxButton.OK, MessageBoxImage.Information);
            db.ThemHoaDon(TongTien, MaKhachHang);
            foreach(Food food in Foods)
            {
                db.ThemCTHD(food);
            }


        }
    }
}
