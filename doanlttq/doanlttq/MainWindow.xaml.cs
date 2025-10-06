using doanlttq;
using doanlttq.MonAn;
using doanlttq.MonAn;
using System.Collections.ObjectModel;
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


public partial class MainWindow : Window
    {

            public List<Food> Foods { get; set; }

            public MainWindow()
            {
                InitializeComponent();
                DatabaseHelper db = new DatabaseHelper();
                Foods = db.GetFoods();
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
                    new GradientStop(Colors.White, 0.81),
                    new GradientStop(Color.FromArgb(0xFF, 0xEE, 0x6E, 0x18), 0.811),
                    new GradientStop(Color.FromArgb(0xFF, 0xEE, 0x6E, 0x18), 1)
                        }
            };
            (sender as Button).Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xEE, 0x6E, 0x18));
            if (((sender as Button).Content).ToString() == "Món Chính")
                MessageBox.Show("Đã chọn món chính");
        }

        private void Them_Mon(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var food = btn.DataContext as Food;
            if (food != null)
                MessageBox.Show($"Bạn đã chọn: {food.TenMA} - Giá: {food.Gia}");
        }
        private void Tim_Kiem_Mon_An(object sender, RoutedEventArgs e)
        {
            string TenMonAn = Textbox_TimKiem.Text;

                DatabaseHelper db = new DatabaseHelper();
                Foods = db.LocMonAn(TenMonAn);
                this.DataContext = null;
                this.DataContext = this;
            
        }
    }
}

