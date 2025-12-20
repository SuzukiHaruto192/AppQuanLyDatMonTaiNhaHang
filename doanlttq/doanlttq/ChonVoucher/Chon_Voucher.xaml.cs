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

namespace doanlttq.ChonVoucher
{
    /// <summary>
    /// Interaction logic for Chon_Voucher.xaml
    /// </summary>
    public partial class Chon_Voucher : Window
    {
        public ObservableCollection<Voucher> Vouchers { get; set; }
        public Chon_Voucher()
        {
            InitializeComponent();
            this.DataContext = this;
            DatabaseHelper db = new DatabaseHelper();
            int diemtl = db.LayDiemTichLuy(((App)Application.Current).MaKH);
            DiemTL.Text = $"{diemtl} ĐIỂM";
            Vouchers =new ObservableCollection<Voucher>( db.LayVoucher(((App)Application.Current).TongTien,diemtl));
            if (Application.Current is App app && app.VoucherApDung != null) // nếu đã có voucher áp dụng
            {
                var v = Vouchers.FirstOrDefault(x => x.MaVoucher == app.VoucherApDung.MaVoucher);
                if (v != null)
                    lv_Vouchers.SelectedItem = v;
            }
        }

        private void Quay_Lai(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Ap_Dung(object sender, RoutedEventArgs e)
        {
            if (lv_Vouchers.SelectedItem is Voucher selectedVoucher )
            {
               ((App)Application.Current).VoucherApDung = selectedVoucher;
            }
            this.Close();
        }
    }
}
