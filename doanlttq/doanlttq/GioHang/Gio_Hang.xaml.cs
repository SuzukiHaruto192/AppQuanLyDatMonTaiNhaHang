using doanlttq;
using doanlttq.MonAn;
using doanlttq.ViewModels;
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

    public partial class Gio_Hang : Window
    {
      
        private OrderViewModel orderViewModel;
        private FoodWatcher _watcher;

        public Gio_Hang(OrderViewModel viewModel)
        {
            InitializeComponent();
            MB.Text = "BÀN " + ((App)Application.Current).MABAN.Substring(1);
            this.orderViewModel = viewModel;
            _watcher = new FoodWatcher(((App)Application.Current).connectionString);

            _watcher.OnDatabaseChanged += () =>
            {
                Dispatcher.Invoke(() => LoadData());
            };

            _watcher.StartListening();
            LoadData();
        }
        private void LoadData()
        {
            orderViewModel.LoadRecommendations();
            this.DataContext = orderViewModel;

            orderViewModel.FilterMenu();
            UpdateUI();
        }


        // Đồng nhất giao diện giữa MainWindow với GioHang khi chuyển qua lại
        private void UpdateUI()
        {
            string target = orderViewModel.LoaiMon;
            switch (target)
            {
                case "L02":
                    target = "Combo";
                    break;
                case "L03":
                    target = "Khai Vị";
                    break;
                case "L04":
                    target = "Món Chính";
                    break;
                case "L06":
                    target = "Tráng Miệng";
                    break;
                case "L07":
                    target = "Nước Uống";
                    break;
                default:
                    target = "Đồ ăn kèm";
                    break;
            }

            foreach (var child in MenuGrid.Children)
            {

                if (child is Button btn)
                {
                    btn.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00));
                    btn.Foreground = Brushes.White;
                    if (btn.Content.ToString() == target)
                    {
                        btn.Background = Brushes.White;
                        btn.Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00));
                    }
                }
            }

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
            {
                orderViewModel.LoaiMon = "L03";
                orderViewModel.CategoryId_Child = new List<string>
                {
                    "L12", "L13", "L14", "L15"
                };
            }
            else if (((sender as Button).Content).ToString() == "Món Chính")
            {
                orderViewModel.LoaiMon = "L04";
                orderViewModel.CategoryId_Child = new List<string>
                {
                    "L16", "L17", "L18", "L19", "L20", "L21", "L22"
                };
            }
            else if (((sender as Button).Content).ToString() == "Tráng Miệng")
            {
                orderViewModel.LoaiMon = "L06";
                orderViewModel.CategoryId_Child = new List<string>
                {
                    "L24", "L25", "L26"
                };
            }
            else if (((sender as Button).Content).ToString() == "Nước Uống")
            {
                orderViewModel.LoaiMon = "L07";
                orderViewModel.CategoryId_Child = new List<string>
                {
                    "L27", "L28"
                };
            }
            else if (((sender as Button).Content).ToString() == "Combo")
            {
                orderViewModel.LoaiMon = "L02";
                orderViewModel.CategoryId_Child = new List<string>
                {
                    "L09", "L10", "L11"
                };
            }
            else
            {
                orderViewModel.LoaiMon = "L08";
                orderViewModel.CategoryId_Child = new List<string>
                {
                    "L29", "L30", "L31"
                };
            }
            Application.Current.Dispatcher.Invoke(() =>
            {
                orderViewModel.FilterMenu();
            });
          }


        private void Tim_Kiem_Mon_An(object sender, RoutedEventArgs e)
        {
            string TenMonAn = Textbox_TimKiem.Text;

            orderViewModel.Tim_Kiem_Mon_An(TenMonAn);
        }

        private void Gio_Hang_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow(orderViewModel);
            mainWindow.Show();
            this.Close();
        }
        private async void Xac_Nhan(object sender, RoutedEventArgs e) 
        {
            await orderViewModel.ExecuteConfirmAsync();
        }
        private void Giam_SoLuong_Mon(object sender, RoutedEventArgs e) 
        {
            Button btn = sender as Button;

            var food = btn.DataContext as Food;
            var container = lstMon.ItemContainerGenerator.ContainerFromItem(food) as FrameworkElement;
            if (container == null)
                return;


            Border borderAnimation = new Border();
            borderAnimation.Height = (container).ActualHeight;
            borderAnimation.Width = (container).ActualWidth;
            borderAnimation.Background = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xA5, 0x00));
            borderAnimation.CornerRadius = new CornerRadius(20);
            borderAnimation.BorderBrush = new SolidColorBrush(Colors.Black);
            borderAnimation.BorderThickness = new Thickness(1);
            ImageBrush img = new ImageBrush();
            img.ImageSource = new BitmapImage(new Uri(food.ANH, UriKind.RelativeOrAbsolute));
            img.Stretch = Stretch.UniformToFill; // Giúp ảnh luôn đầy khung
            borderAnimation.Background = img;


            Point startPoint = container.TranslatePoint(new Point(0, 0), this);
            Canvas_Animation.Children.Add(borderAnimation);
            Canvas.SetLeft(borderAnimation, startPoint.X);
            Canvas.SetTop(borderAnimation, startPoint.Y);

            double offsetX = (borderAnimation.Width - 30) / 2;
            double offsetY = (borderAnimation.Height - 30) / 2;

            double newX = startPoint.X + offsetX;
            double newY = startPoint.Y + offsetY;

            DoubleAnimation AnimationThuNhoHeight = new DoubleAnimation();
            AnimationThuNhoHeight.From = borderAnimation.Height;
            AnimationThuNhoHeight.To = 30;
            AnimationThuNhoHeight.Duration = TimeSpan.FromSeconds(0.1);
            AnimationThuNhoHeight.AutoReverse = false;

            DoubleAnimation animationThuNhoWidth = new DoubleAnimation();
            animationThuNhoWidth.From = borderAnimation.Width;
            animationThuNhoWidth.To = 30;
            animationThuNhoWidth.Duration = TimeSpan.FromSeconds(0.1);
            animationThuNhoWidth.AutoReverse = false;

            DoubleAnimation animLeft = new DoubleAnimation(startPoint.X, newX, TimeSpan.FromSeconds(0.1));
            DoubleAnimation animTop = new DoubleAnimation(startPoint.Y, newY, TimeSpan.FromSeconds(0.1));


            animLeft.Completed += (s, e) =>
            {
                Point EndPoint = bnt_GH.TranslatePoint(new Point(0, 0), this);
                double DestPointX = EndPoint.X + (bnt_GH.ActualWidth / 2) - 15;
                double DestPointY = EndPoint.Y + (bnt_GH.ActualHeight / 2) - 15;
                DoubleAnimation endAnimationX = new DoubleAnimation(newX, DestPointX, TimeSpan.FromSeconds(0.8));
                DoubleAnimation endAnimationY = new DoubleAnimation(newY, DestPointY, TimeSpan.FromSeconds(0.8));

                endAnimationX.EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };
                endAnimationY.EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };

                endAnimationX.Completed += (s1, e1) => {
                    Canvas_Animation.Children.Remove(borderAnimation);
                    bnt_GH.RenderTransformOrigin = new Point(0.5, 0.5);
                    RotateTransform rt = new RotateTransform();
                    bnt_GH.RenderTransform = rt;

                    DoubleAnimation RungGio = new DoubleAnimation();
                    RungGio.From = 0;
                    RungGio.To = 10;
                    RungGio.Duration = TimeSpan.FromSeconds(0.02);
                    RungGio.AutoReverse = true;
                    RungGio.RepeatBehavior = new RepeatBehavior(5);

                    rt.BeginAnimation(RotateTransform.AngleProperty, RungGio);
                };

                borderAnimation.BeginAnimation(Canvas.LeftProperty, endAnimationX);
                borderAnimation.BeginAnimation(Canvas.TopProperty, endAnimationY);




            };
            borderAnimation.BeginAnimation(Border.HeightProperty, AnimationThuNhoHeight);
            borderAnimation.BeginAnimation(Border.WidthProperty, animationThuNhoWidth);
            borderAnimation.BeginAnimation(Canvas.LeftProperty, animLeft);
            borderAnimation.BeginAnimation(Canvas.TopProperty, animTop);
            if (food != null)
            {
                orderViewModel.Giam_So_Luong_Mon(food);
            }
        }
        private void Thanh_Toan_Click(object sender, RoutedEventArgs e)
        {
            HoaDon hoaDon = new HoaDon(orderViewModel);
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
                orderViewModel.ThemGioHang(food);
            }
            lstMon.SelectedItem = null;
        }

        private void XoaGio(object sender, RoutedEventArgs e)
        {
            orderViewModel.Xoa_Gio_Hang();
        }

        //private void ThemMon(object sender, RoutedEventArgs e)
        //{
        //    if(sender is Button bnt)
        //    {
        //        var food = bnt.DataContext as Food;
        //        if (food != null)
        //        {
        //            Food tonTai = null;
        //            foreach (Food i in ThemMonAn)
        //            {
        //                if (i.MAMON == food.MAMON)
        //                {
        //                    tonTai = i;
        //                    break;
        //                }
        //            }

        //            if (tonTai == null)
        //            {
        //                food.SoLuong = 1;
        //                ThemMonAn.Add(food);
        //            }
        //            else
        //            {
        //                food.SoLuong++;
        //            }
        //        }
        //    }
        //}
    }
}

