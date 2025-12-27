using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using QuanLyBan.Menu;

namespace QuanLyBan
{
    public partial class UcMenu : UserControl
    {
        public MenuViewModel ViewModel { get; set; }
        private string selectedPath;
        private string relativePath;
        private DatabaseHelper db;
        public UcMenu()
        {
            InitializeComponent();

            db = new DatabaseHelper();
            var list = db.GetListCategory();
            foreach (string category in list)
            { 
                cbCategory.Items.Add(category);
            }

            ViewModel = new MenuViewModel();
            this.DataContext = ViewModel;
        }
        private string ChuanHoaLoaiMon(string category) {
             switch(category)
            {
                case "Súp":
                    return "L12";
                case "Salad":
                    return "L13";
                case "Món Chiên":
                    return "L14";
                case "Món Cuốn":
                    return "L15";
                case "Thịt":
                    return "L16";
                case "Hải Sản":
                    return "L17";
                case "Chay":
                    return "L18";
                case "Lẩu Thịt":
                    return "L19";
                case "Lẩu Hải Sản":
                    return "L20";
                case "Lẩu Chay":
                    return "L21";
                case "Lẩu Đặc Biệt":
                    return "L22";
                case "Trái Cây Tươi":
                    return "L23";
                case "Chè":
                    return "L24";
                case "Kem":
                    return "L25";
                case "Bánh Âu/ Á":
                    return "L26";
                case "Có Cồn":
                    return "L27";
                case "Không Có Cồn":
                    return "L28";
                case "Tinh Bột":
                    return "L29";
                case "Rau/ Nấm":
                    return "L30";
                default:
                    return "L31";
            }
        }
        private void ChonAnh_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "Ảnh (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg";

            if (dialog.ShowDialog() == true)
            {
                selectedPath = dialog.FileName;

                //Copy vao thu muc Images trong project
                string imagesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                if (!System.IO.Directory.Exists(imagesDir))
                {
                    System.IO.Directory.CreateDirectory(imagesDir);
                }
                string fileName = System.IO.Path.GetFileName(selectedPath);
                string destPath = System.IO.Path.Combine(imagesDir, fileName);

                //int count = 1;

                //while (File.Exists(destPath))
                //{
                //    string nameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
                //    string ext = System.IO.Path.GetExtension(fileName);
                //    destPath = System.IO.Path.Combine(imagesDir, $"{nameWithoutExt}_{count}{ext}");
                //    count++;
                //}

                File.Copy(selectedPath, destPath);

                relativePath = System.IO.Path.Combine(System.IO.Path.GetFileName(destPath));
                txtHinh.Text = selectedPath;
            }
        }

        private async void ThemMon_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenMon.Text) || string.IsNullOrWhiteSpace(txtGia.Text) || string.IsNullOrWhiteSpace(txtHinh.Text) 
                 || cbCategory.SelectedItem == null )
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin!");
                return;
            }

            int gia;
            if (!int.TryParse(txtGia.Text, out gia))
            { 
                MessageBox.Show("Giá phải là một số hợp lệ!");
                return;
            }
            string mamon = ChuanHoaMaMon();
            string categoryID = ChuanHoaLoaiMon(cbCategory.SelectedItem.ToString());

            var ai = new MonAnAIServices();

            var Tags = await ai.GetTags(txtTenMon.Text);

            MonAn monAn = new MonAn
            {
                MaMon = mamon,
                Ten = txtTenMon.Text,
                Gia = gia,
                HinhAnh = relativePath,
                TagTinhChat = Tags.TagTinhChat,
                TagMucDich = Tags.TagMucDich,
                MoTa = Tags.MoTa,
                Category = categoryID,
            };
            ViewModel.DanhSachMon.Add(monAn);
            DatabaseHelper db =  new DatabaseHelper();
            db.ThemMonAn(monAn);

            txtTenMon.Clear();
            txtGia.Clear(); ;
            txtHinh.Clear();
            cbCategory.SelectedItem = null;
            selectedPath = null;
        }

        private string ChuanHoaMaMon()
        {
              string data = db.GetMaMon();
              string MaMon = "M" + (Convert.ToInt32(data.Substring(1)) + 1).ToString() ;
              return MaMon;
        }
        private void XoaMon_Click(object sender, RoutedEventArgs e)
        {
            if (lstMon.SelectedItem is MonAn mon)
            {
                ViewModel.DanhSachMon.Remove(mon);
                db.XoaMon(mon.MaMon);
            }
            else
                MessageBox.Show("Hãy chọn món cần xóa!");
        }
    }
}
