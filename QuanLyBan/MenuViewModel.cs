using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBan
{
    public class MenuViewModel
    {
        public ObservableCollection<MonAn> DanhSachMon { get; set; }

        public MenuViewModel()
        {
            DanhSachMon = new ObservableCollection<MonAn>()
        {
            new MonAn(){ Ten="Phở bò", Gia=35000, HinhAnh="Images/pho.jpg" },
            new MonAn(){ Ten="Bún chả", Gia=40000, HinhAnh="Images/buncha.jpg" }
        };
        }
    }
}
