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

            // Tạo dữ liệu mẫu
          Foods = new List<Food>
{
    new Food { Name = "Burger", Price = "50.000đ", ImagePath = "MonAn/AnhMonAn/Burger.jpg" },
    new Food { Name = "coca", Price = "40.000đ", ImagePath = "MonAn/AnhMonAn/coca.png" },
    new Food { Name = "Cơm Tấm", Price = "70.000đ", ImagePath = "MonAn/AnhMonAn/ComTam.jpg" },
};

            this.DataContext = this; // rất quan trọng: để XAML thấy property Foods
        }
        private void Click_Menu(object sender, RoutedEventArgs e)
        {
            // chỉ duyệt trong MenuGrid
            foreach (var child in MenuGrid.Children)
            {
                if (child is Button btn)
                    btn.Background = Brushes.White; // reset màu
            }

            (sender as Button).Background = new LinearGradientBrush
            {
                StartPoint = new Point(0.5, 0),
                EndPoint = new Point(0.5, 1),
                GradientStops = new GradientStopCollection
                {
                    new GradientStop(Colors.White, 0),
                    new GradientStop(Colors.White, 0.81),
                    new GradientStop(Colors.Black, 0.811),
                    new GradientStop(Colors.Black, 1)
                        }
            };
            if (((sender as Button).Content).ToString() == "Món Chính")
                MessageBox.Show("Đã chọn món chính");
        }

        private void Them_Mon(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            var food = btn.DataContext as Food;
            if (food != null)
                MessageBox.Show($"Bạn đã chọn: {food.Name} - Giá: {food.Price}");
        }
    }
}

