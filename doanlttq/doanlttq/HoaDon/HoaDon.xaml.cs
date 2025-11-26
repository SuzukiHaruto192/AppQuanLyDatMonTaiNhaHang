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
using System.Collections.Specialized;
using System.ComponentModel;

namespace doanlttq
{
    /// <summary>
    /// Interaction logic for HoaDon.xaml
    /// </summary>
    public partial class HoaDon : Window 
    {
        public ObservableCollection<Food> Foods { get; set; }

        public HoaDon()
        {
            InitializeComponent();
            DatabaseHelper db= new DatabaseHelper();
            Foods = db.LayCTHD();
            Foods.CollectionChanged += Foods_CollectionChanged;
            TinhLaiTongTien();

            this.DataContext = this;
        }
        private void Foods_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            TinhLaiTongTien();   
        }

        private void TinhLaiTongTien()
        {
            decimal tong = 0;
            if (Foods != null)
            {
                foreach (Food item in Foods)
                {
                    tong += item.GIA * item.SoLuong;
                }
            }
            ((App)Application.Current).TongTien = tong;

           Tong_Tien.Text = string.Format("{0:N0} VNĐ", tong);
        }
        private void Quay_Lai(object sender, RoutedEventArgs e) 
        { 
            Gio_Hang gh= new Gio_Hang();
            gh.Show();
            this.Close();
        }
        private void Thanh_Toan(object sender, RoutedEventArgs e)
        {
            DatabaseHelper db = new DatabaseHelper();
            string MaKhachHang = Interaction.InputBox("Nhập Số Điện Thoại", "Tích Điểm", "");

            if (string.IsNullOrWhiteSpace(MaKhachHang) || MaKhachHang.Length > 10)
            {
                MaKhachHang = "0";
            }
            else {
                if (db.TimMAKH(MaKhachHang) == false)
                {
                    //MessageBox.Show($"Mã Khách Hàng Mới : {MaKhachHang} Quét QR để thanh toán");
                    db.ThemKhachHang(MaKhachHang);
                }
                //else
                    //MessageBox.Show($"Mã Khách Hàng: {MaKhachHang} Quét QR để thanh toán");
            }
            BitmapImage qrImage = Qr.TaoQr(
                    NganHang: "VCB",
                    STK: "9706101617",
                    ChuTaiKhoan: "LE DUY QUANG",
                    SoTien: ((App)Application.Current).TongTien,
                    GhiChu: "Thanh toan don hang #123"
                );

            ((App)Application.Current).GioRa = DateTime.Now;
            HienQrThanhToan qrtt = new HienQrThanhToan(qrImage);
            bool? ketQua = qrtt.ShowDialog();
            if (ketQua == true)
                MessageBox.Show("Đơn hàng đã thanh toán thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                //MessageBox.Show("Đơn hàng đã thanh toán Thất bại!", "Thông báo",

                //MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            db.UpdateHoaDon(((App)Application.Current).TongTien, MaKhachHang,((App)Application.Current).GioRa,"Đã Thanh Toán");
            db.KhachDi(((App)Application.Current).MABAN);
            this.Close();


        }

        private void Tien_Mat(object sender, RoutedEventArgs e)
        {
            DatabaseHelper db = new DatabaseHelper();
            string MaKhachHang = Interaction.InputBox("Nhập Số Điện Thoại", "Tích Điểm", "");

            if (string.IsNullOrWhiteSpace(MaKhachHang) || MaKhachHang.Length > 10)
            {
                MaKhachHang = "0";
            }
            else
            {
                if (db.TimMAKH(MaKhachHang) == false)
                {
                    //MessageBox.Show($"Mã Khách Hàng Mới : {MaKhachHang} Quét QR để thanh toán");
                    db.ThemKhachHang(MaKhachHang);
                }
                //else
                //MessageBox.Show($"Mã Khách Hàng: {MaKhachHang} Quét QR để thanh toán");
            }
            ((App)Application.Current).GioRa = DateTime.Now;

            db.UpdateHoaDon(((App)Application.Current).TongTien, MaKhachHang, ((App)Application.Current).GioRa, "Chưa Thanh Toán");
            login lg = new login();
            lg.Show();
            this.Close();
        }
    }
}
