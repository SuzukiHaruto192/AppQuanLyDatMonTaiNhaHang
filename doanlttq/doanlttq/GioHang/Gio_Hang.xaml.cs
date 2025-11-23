using doanlttq;
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


    public partial class Gio_Hang : Window
    {

        public List<Food> Foods { get; set; }
        public ObservableCollection<Food> ThemMonAn { get; set; }
        public ObservableCollection<Food> HD { get; set; }
        private string loaimon = "L03";

        public Gio_Hang(ObservableCollection<Food> TMA)
        {
            InitializeComponent();
            DatabaseHelper db = new DatabaseHelper();
            Foods = db.LocMonAnChuDe( loaimon);
            ThemMonAn = TMA;
            HD = new ObservableCollection<Food>();
            this.DataContext = this; 
        }
        public Gio_Hang(ObservableCollection<Food> TMA, ObservableCollection<Food> hd)
        {
            InitializeComponent();
            DatabaseHelper db = new DatabaseHelper();
            Foods = db.LocMonAnChuDe( loaimon);
            HD= hd;
            ThemMonAn = new ObservableCollection<Food>();
            this.DataContext = this;
        }
        public Gio_Hang(ObservableCollection<Food> TMA, ObservableCollection<Food> TMA1, ObservableCollection<Food> hd)
        {
            InitializeComponent();
            DatabaseHelper db = new DatabaseHelper();
            Foods = db.LocMonAnChuDe( loaimon);
            ThemMonAn = TMA;
            if (hd == null)
            {
                HD = new ObservableCollection<Food>();
            }
            else
            {
                HD = hd; 
            }
            this.DataContext = this;
        }

        private void Click_Menu(object sender, RoutedEventArgs e)
        {
            // chỉ duyệt trong MenuGrid
            foreach (var child in MenuGrid.Children)
            {
                if (child is Button btn)
                {
                    btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00));
                    btn.Foreground = Brushes.White;

                } // reset màu
            }

            (sender as Button).Background = Brushes.White;
            (sender as Button).Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00));
            if (((sender as Button).Content).ToString() == "Khai Vị")
                loaimon = "L03";
            else if (((sender as Button).Content).ToString() == "Món Chính")
                loaimon = "L04";
            else if (((sender as Button).Content).ToString() == "Tráng Miệng")
                loaimon = "L06";
            else if (((sender as Button).Content).ToString() == "Nước Uống")
                loaimon = "L07";
            else if (((sender as Button).Content).ToString() == "Combo")
                loaimon = "L02";
            else
                loaimon = "L08";
            DatabaseHelper db = new DatabaseHelper();
            Foods = db.LocMonAnChuDe(loaimon);
            this.DataContext = null;
            this.DataContext = this;
        }

        private void Them_Mon(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var food = btn.DataContext as Food;
            if (food != null)
            {
                Food tonTai = null;
                foreach (Food i in ThemMonAn)
                {
                    if (i.MAMON == food.MAMON)
                    {
                        tonTai = i;
                        break;
                    }
                }

                if (tonTai == null)
                {
                    food.SoLuong = 1;
                    ThemMonAn.Add(food);
                }
                else
                {
                    tonTai.SoLuong++;
                    ThemMonAn.Remove(tonTai);
                    ThemMonAn.Add(tonTai);
                }
            }
        }

        private void Tim_Kiem_Mon_An(object sender, RoutedEventArgs e)
        {
            string TenMonAn = Textbox_TimKiem.Text;

            DatabaseHelper db = new DatabaseHelper();
            Foods = db.TimMon(TenMonAn, loaimon);
            this.DataContext = null;
            this.DataContext = this;

        }
        private void Gio_Hang_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(ThemMonAn, HD);
            mainWindow.Show();
            this.Close();
        }
        private void Xac_Nhan(object sender, RoutedEventArgs e) 
        {
            if (ThemMonAn != null && ThemMonAn.Count > 0)
            {
                foreach (var food in ThemMonAn)
                {
                    ((App)Application.Current).TongTien += food.GIA;
                    bool kt= true;
                    foreach(Food i in HD)
                    {
                        if (food.MAMON == i.MAMON)
                        {
                            i.SoLuong+=food.SoLuong;
                            kt= false;
                        }
                    }
                    if (kt)
                        HD.Add(food);
                }
                if (((App)Application.Current).ThemHDFirst == true)
                {
                    DatabaseHelper db = new DatabaseHelper();
                    db.ThemHoaDon(((App)Application.Current).TongTien, "0");
                    ((App)Application.Current).ThemHDFirst = false;
                }
                else
                {
                    DatabaseHelper db = new DatabaseHelper();
                    db.UpdateHoaDon(((App)Application.Current).TongTien, "0", null , "Chưa thanh toán");
                }
                DatabaseHelper db2 = new DatabaseHelper();
                foreach (var food in ThemMonAn)
                db2.ThemCTHD(food);
                ThemMonAn.Clear();
            }
        }
        private void Giam_SoLuong_Mon(object sender, RoutedEventArgs e) 
        {
          Button btn = sender as Button;
            var food= btn.DataContext as Food;
            if (food != null) {
                food.SoLuong--;
                ThemMonAn.Remove(food);
                if (food.SoLuong != 0) { 
                 ThemMonAn.Add(food);
                }
            }
        }
        private void Thanh_Toan_Click(object sender, RoutedEventArgs e)
        {
            Xac_Nhan(sender,e);
            HoaDon hoaDon = new HoaDon(HD);
            hoaDon.Show();
            this.Close();
        }
        private void Textbox_TimKiem_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Textbox_TimKiem.Text))
            {
                Textbox_TimKiem.Background = new SolidColorBrush(Color.FromArgb(0x00, 0xFF, 0xA5, 0x00));
            }
            else
            {
                Textbox_TimKiem.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00));
            }
        }

        private void lstMon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var food = lstMon.SelectedItem as Food;
            if (food != null)
            {
                Food tonTai = null;
                foreach (Food i in ThemMonAn)
                {
                    if (i.MAMON == food.MAMON)
                    {
                        tonTai = i;
                        break;
                    }
                }

                if (tonTai == null)
                {
                    food.SoLuong = 1;
                    ThemMonAn.Add(food);
                }
                else
                {
                    tonTai.SoLuong++;
                    ThemMonAn.Remove(tonTai);
                    ThemMonAn.Add(tonTai);
                }
            }
            lstMon.SelectedItem = null;
        }

        private void XoaGio(object sender, RoutedEventArgs e)
        {
            ThemMonAn.Clear();
        }


    }
}

