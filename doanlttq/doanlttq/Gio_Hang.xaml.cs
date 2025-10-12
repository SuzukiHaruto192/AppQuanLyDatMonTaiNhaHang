using doanlttq.MonAn;
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
    public partial class Gio_Hang : Window
    {
        public ObservableCollection<Food> Foods { get; set; }

        public Gio_Hang(ObservableCollection<Food> foods)
        {
            InitializeComponent();
            Foods = foods;
            Foods.CollectionChanged += Foods_CollectionChanged;
            this.DataContext = this;
            int TongTien = 0;
            foreach (Food food in Foods)
            {
                TongTien += food.GIA;
            }
            Tong_Tien.Text = "Tổng Tiền :" + TongTien;
        }
        private void Foods_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            int TongTien = 0;
            foreach (Food food in Foods)
            {
                TongTien += food.GIA;
            }
            Tong_Tien.Text = "Tổng Tiền :" + TongTien;
        }

        private void Xoa_Khoi_GH(object sender, RoutedEventArgs e)
        {
            Button bnt= sender as Button;
            var MonXoa = bnt.DataContext as Food;
            if( MonXoa != null ) 
                Foods.Remove( MonXoa );
        }
        private void Thanh_Toan(object sender, RoutedEventArgs e)
        {
        }
    }
}
