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

namespace QuanLyBan
{
    public partial class MainWindow : Window
    {
        private UcBan _viewBan;
        private UcMenu _viewMenu;
        private UcThongKe _viewThongKe;

        public MainWindow()
        {
            InitializeComponent();

            _viewBan = new UcBan();
            _viewMenu = new UcMenu();
            _viewThongKe = new UcThongKe();

            MainContent.Content = _viewMenu;
        }

        private void cbbNavigation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;

            if (comboBox == null || comboBox.SelectedItem == null || MainContent == null) return;

            var selectedItem = comboBox.SelectedItem as ComboBoxItem;
            if (selectedItem.Tag == null) return;

            string tag = selectedItem.Tag.ToString();

            switch (tag)
            {
                case "QuanLyBan":
                    MainContent.Content = _viewBan; 
                    break;
                case "QuanLyMenu":
                    MainContent.Content = _viewMenu;
                    break;
                case "QuanLyThongKe":
                    MainContent.Content = _viewThongKe;
                    break;
            }
        }
    }
}
