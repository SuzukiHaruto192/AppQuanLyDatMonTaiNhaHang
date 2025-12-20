using doanlttq.ChonVoucher;
using doanlttq.MonAn;
using doanlttq.Qrcode;
using doanlttq.ViewModels;
using Microsoft.VisualBasic;
using PayOS;
using PayOS.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
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
    /// Interaction logic for HoaDon.xaml
    /// </summary>
    public partial class HoaDon : Window 
    {
        public ObservableCollection<Food> Foods { get; set; }

        private OrderViewModel orderViewModel;

        public HoaDon(OrderViewModel viewModel)
        {
            InitializeComponent();
            this.orderViewModel = viewModel;
            DatabaseHelper db = new DatabaseHelper();
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

            // TÍCH ĐIỂM CÓ CHỈNH SỬA
            Voucher_Giam.Text = ((App)Application.Current).VoucherApDung != null ?
                $"- {((App)Application.Current).VoucherApDung.GiaTriGiam:N0} đ" : "";
            Tong_Tien.Text = string.Format("{0:N0} VNĐ", tong - (((App)Application.Current).VoucherApDung?.GiaTriGiam ?? 0));
            // END TÍCH ĐIỂM
        }
        private void Quay_Lai(object sender, RoutedEventArgs e) 
        { 
            Gio_Hang gh= new Gio_Hang(orderViewModel);
            gh.Show();
            this.Close();
        }
        private void Thanh_Toan(object sender, RoutedEventArgs e)
        {
            if (((App)Application.Current).TongTien == 0)
                return;
            DatabaseHelper db = new DatabaseHelper();
            HienQrThanhToan qrtt = new HienQrThanhToan();
            bool? ketQua = qrtt.ShowDialog();
            if (ketQua == true)
                MessageBox.Show("Đơn hàng đã thanh toán thành công!", "Thông báo",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            else
            {
                return;
            }
                        ((App)Application.Current).GioRa = DateTime.Now;
            // TÍCH ĐIỂM
            db.UpdateHoaDon(((App)Application.Current).TongTien, ((App)Application.Current).GioRa, "Đã Thanh Toán", ((App)Application.Current).VoucherApDung?.GiaTriGiam ?? 0);
            if (((App)Application.Current).MaKH != "0") // Tích điểm nếu ban đầu có nhập sdt
            {
                int diemTichLuy = (int)(((App)Application.Current).TongTien / 10000);
                db.TichDiem(((App)Application.Current).MaKH, diemTichLuy - (((App)Application.Current).VoucherApDung?.SoDiem ?? 0));
            }
            // END TÍCH ĐIỂM
            db.KhachDi(((App)Application.Current).MABAN);
            this.Close();


        }

        private void Tien_Mat(object sender, RoutedEventArgs e)
        {
            if (((App)Application.Current).TongTien == 0)
                return;
            DatabaseHelper db = new DatabaseHelper();
            ((App)Application.Current).GioRa = DateTime.Now;
            // TÍCH ĐIỂM
            db.UpdateHoaDon(((App)Application.Current).TongTien, ((App)Application.Current).GioRa, "Đã Thanh Toán", ((App)Application.Current).VoucherApDung?.GiaTriGiam ?? 0);
            if (((App)Application.Current).MaKH != "0") // Tích điểm nếu ban đầu có nhập sdt
            {
                int diemTichLuy = (int)(((App)Application.Current).TongTien / 10000);
                db.TichDiem(((App)Application.Current).MaKH, diemTichLuy - (((App)Application.Current).VoucherApDung?.SoDiem ?? 0));
            }
            // END TÍCH ĐIỂM
            db.KhachDi(((App)Application.Current).MABAN);
            login lg = new login();
            lg.Show();
            this.Close();
        }
        private void dp_Voucher_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) //TÍCH ĐIỂM
        {
            Chon_Voucher cv = new Chon_Voucher();
            cv.ShowDialog();
            // Cập nhật lại tổng tiền sau khi áp dụng voucher
            Voucher_Giam.Text = ((App)Application.Current).VoucherApDung != null ?
                $"- {((App)Application.Current).VoucherApDung.GiaTriGiam:N0} đ" : "";
            Tong_Tien.Text = string.Format("{0:N0} VNĐ", ((App)Application.Current).TongTien - (((App)Application.Current).VoucherApDung?.GiaTriGiam ?? 0));
        }
    }
}
