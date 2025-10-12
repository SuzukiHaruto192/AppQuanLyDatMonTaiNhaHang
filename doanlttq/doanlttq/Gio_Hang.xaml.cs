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
    public partial class Gio_Hang : Window
    {
        public ObservableCollection<Food> Foods { get; set; }

        public Gio_Hang(ObservableCollection<Food> foods)
        {
            InitializeComponent();
            Foods = foods;
            this.DataContext = this;
        }


        private void Xoa_Khoi_GH(object sender, RoutedEventArgs e)
        {
            string GiamSoLuong = Interaction.InputBox("", "Nhập Số Lượng Món Cần Xoá ", "1");
            Button bnt= sender as Button;
            var MonXoa = bnt.DataContext as Food;
            if (int.TryParse(GiamSoLuong, out int so))
            {
                if (so <= MonXoa.SoLuong)
                {
                    if (MonXoa != null)
                    {
                        MonXoa.SoLuong = MonXoa.SoLuong-so;
                        if(MonXoa.SoLuong ==0)
                            Foods.Remove(MonXoa);
                        else
                        {
                            Foods.Remove(MonXoa);
                            Foods.Add(MonXoa);
                        }
                    }
                }
                else
                    MessageBox.Show("Số Lượng Món Xoá Không Hợp Lệ");
            }
            else
                MessageBox.Show("Số Lượng Món Xoá Không Hợp Lệ");
        }
        private void Xac_Nhan(object sender, RoutedEventArgs e)
        {
            HoaDon hd = new HoaDon(Foods);
            hd.Show();
        }
    }
}
