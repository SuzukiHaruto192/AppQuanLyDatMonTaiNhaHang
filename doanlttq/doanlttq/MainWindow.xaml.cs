using doanlttq;
using doanlttq.MonAn;
using doanlttq.MonAn;
using Microsoft.VisualBasic;
using QRCoder;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


namespace doanlttq
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// public class Food
    ///     public class QrTest


    public partial class MainWindow : Window
    {

            public List<Food> Foods { get; set; }
        public ObservableCollection<Food> ThemMonAn;
        private string loaimon = "L03";

            public MainWindow()
            {
                InitializeComponent();
                DatabaseHelper db = new DatabaseHelper();
            Foods = db.LocMonAn("", loaimon);
            ThemMonAn = new ObservableCollection<Food>();
                this.DataContext = this;
        }

        private void Click_Menu(object sender, RoutedEventArgs e)
        {
            // chỉ duyệt trong MenuGrid
            foreach (var child in MenuGrid.Children)
            {
                if (child is Button btn)
                {
                    btn.Background = Brushes.White;
                    btn.Foreground = Brushes.Black;
                } // reset màu
            }

            (sender as Button).Background = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Colors.White, 0),
                    new GradientStop(Colors.White, 0.91),
                    new GradientStop(Color.FromArgb(0xFF, 0xEE, 0x6E, 0x18), 0.911),
                    new GradientStop(Color.FromArgb(0xFF, 0xEE, 0x6E, 0x18), 1)
                        }
            };
            (sender as Button).Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xEE, 0x6E, 0x18));
            if (((sender as Button).Content).ToString() == "Khai Vị")
                loaimon = "L03";
            else if (((sender as Button).Content).ToString() == "Món Chính")
                loaimon = "L04";
            else if (((sender as Button).Content).ToString() == "Tráng Miệng")
                loaimon = "L06";
            else if (((sender as Button).Content).ToString() == "Nước Uống")
                loaimon = "L07";
            else
                loaimon = "L08";
            DatabaseHelper db = new DatabaseHelper();
            Foods = db.LocMonAn("", loaimon);
            this.DataContext = null;
            this.DataContext = this;
        }

        private void Them_Mon(object sender, RoutedEventArgs e)
        {
            string SoLuongMon = Interaction.InputBox("", "Nhập Số Lượng Món Ăn Đã Chọn", "1");
            Button btn = sender as Button;
            var food = btn.DataContext as Food;
            if (int.TryParse(SoLuongMon, out int so))
            {

                // ✅ Mới:
                if (food != null)
                {
                    bool kt=true;
                    int tam = 0;
                    foreach (Food i in ThemMonAn) {
                        if (i.MAMON == food.MAMON)
                        {
                            kt=false;
                            tam = i.SoLuong + so;
                        }
                    }
                    if (kt)
                    {
                        food.SoLuong = so;
                        ThemMonAn.Add(food);
                    }
                    else
                    {
                        ThemMonAn.Remove(food);
                        food.SoLuong = tam;
                        ThemMonAn.Add(food);
                    }
                }
            }
            else
            {
                MessageBox.Show("Giá trị không hợp lệ! Vui lòng nhập số.");
            }
        }
        private void Tim_Kiem_Mon_An(object sender, RoutedEventArgs e)
        {
            string TenMonAn = Textbox_TimKiem.Text;

                DatabaseHelper db = new DatabaseHelper();
                Foods = db.LocMonAn(TenMonAn,loaimon);
                this.DataContext = null;
                this.DataContext = this;
            
        }
        private void Gio_Hang_Click (object sender, RoutedEventArgs e)
        {
            Gio_Hang gioHang = new Gio_Hang(ThemMonAn);
            gioHang.Show();
        }
    }
}

