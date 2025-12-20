using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace QuanLyBan.Menu
{
    public class MenuViewModel
    {
        public ObservableCollection<MonAn> DanhSachMon { get; set; }

        public MenuViewModel()
        {
            DanhSachMon = new ObservableCollection<MonAn>();

            DatabaseHelper db = new DatabaseHelper();
            var listMon = db.GetListMonAn();

            foreach (var item in listMon)
            {
                DanhSachMon.Add(item);
            }
        }
    }
}
