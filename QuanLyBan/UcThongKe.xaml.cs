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

namespace QuanLyBan
{
    public partial class UcThongKe : UserControl
    {
        public UcThongKe()
        {
            InitializeComponent();

            MainView.Content = new Daylyview();
        }

        private void btnDayly_Click(object sender, RoutedEventArgs e)
        {
            MainView.Content = new Daylyview();
        }

        private void btnMonthly_Click(object sender, RoutedEventArgs e)
        {
            MainView.Content = new MonthlyView();
        }
    }
}

