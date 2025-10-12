using doanlttq.MonAn;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace doanlttq
{
    /// <summary>
    /// Interaction logic for HoaDon.xaml
    /// </summary>
    public partial class HoaDon : Window
    {
        public ObservableCollection<Food> Foods { get; set; }
        public HoaDon(ObservableCollection<Food> foods)
        {
            InitializeComponent();
            Foods = foods;
            this.DataContext = this;
            int TongTien = 0;
            foreach (Food Food in foods)
            {
                TongTien += Food.GIA * Food.SoLuong;
            }
            Tong_Tien.Text="Tổng Tiền : "+TongTien;
        }
    }
}
