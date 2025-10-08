using Microsoft.Win32;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuanLyMenu
{

    public partial class MainWindow : Window
    {
        public MenuViewModel ViewModel { get; set; }
        private string duongDanAnh;
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new MenuViewModel();
            this.DataContext = ViewModel;
        }

        private void ChonAnh_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Ảnh (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                duongDanAnh = dialog.FileName;
            }
            txtHinh.Text = duongDanAnh;
        }

        private void ThemMon_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenMon.Text) || string.IsNullOrWhiteSpace(txtGia.Text) || string.IsNullOrEmpty(duongDanAnh))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin và chọn ảnh!");
                return;
            }

            ViewModel.DanhSachMon.Add(new MonAn
            {
                Ten = txtTenMon.Text,
                Gia = int.Parse(txtGia.Text),
                HinhAnh = duongDanAnh
            });

            txtTenMon.Clear();
            txtGia.Clear();;
            txtHinh.Clear();
            duongDanAnh = null;
        }

        private void XoaMon_Click(object sender, RoutedEventArgs e)
        {
            if (lstMon.SelectedItem is MonAn mon)
                ViewModel.DanhSachMon.Remove(mon);
            else
                MessageBox.Show("Hãy chọn món cần xóa!");
        }
    }
}
    