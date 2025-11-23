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

namespace QuanLyBan
{
    public partial class UcMenu : UserControl
    {
        public MenuViewModel ViewModel { get; set; }
        private string selectedPath;
        private string relativePath;
        public UcMenu()
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
                selectedPath = dialog.FileName;
                string imagesDir = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");

                if (!System.IO.Directory.Exists(imagesDir))
                {
                    System.IO.Directory.CreateDirectory(imagesDir);
                }
                string fileName = System.IO.Path.GetFileName(selectedPath);
                string destPath = System.IO.Path.Combine(imagesDir, fileName);

                int count = 1;

                while (File.Exists(destPath))
                {
                    string nameWithoutExt = System.IO.Path.GetFileNameWithoutExtension(fileName);
                    string ext = System.IO.Path.GetExtension(fileName);
                    destPath = System.IO.Path.Combine(imagesDir, $"{nameWithoutExt}_{count}{ext}");
                    count++;
                }

                File.Copy(selectedPath, destPath);

                relativePath = System.IO.Path.Combine("Images", System.IO.Path.GetFileName(destPath));
                txtHinh.Text = selectedPath;
            }
        }

        private void ThemMon_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtTenMon.Text) || string.IsNullOrWhiteSpace(txtGia.Text) || string.IsNullOrEmpty(txtHinh.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin và chọn ảnh!");
                return;
            }

            ViewModel.DanhSachMon.Add(new MonAn
            {
                Ten = txtTenMon.Text,
                Gia = int.Parse(txtGia.Text),
                HinhAnh = relativePath
            });

            txtTenMon.Clear();
            txtGia.Clear(); ;
            txtHinh.Clear();
            selectedPath = null;
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
