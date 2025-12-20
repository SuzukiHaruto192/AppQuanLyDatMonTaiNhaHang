using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace doanlttq.MonAn
{

        public class Food :INotifyPropertyChanged
    {
            public string MAMON { get; set; }
            public string TENMON { get; set; }
            public decimal GIA { get; set; }
            public string ANH { get; set; }
            public string MOTA {  get; set; }
            public List<string> TagsTinhChat { get; set; } = new List<string>();
            public List<string> TagsMucDich { get; set; } = new List<string>();
            public string CATEGORYID { get; set; }
            private int _soLuong;
            public int score { get; set; } = 0;
        public int SoLuong
        {
            get { return _soLuong; }
            set
            {
                if (_soLuong != value)
                {
                    _soLuong = value;
                    // 3. Thông báo SoLuong thay đổi (để cập nhật ô số lượng trên UI)
                    OnPropertyChanged();

                    // 4. QUAN TRỌNG NHẤT: Thông báo THANHTIEN cũng thay đổi theo
                    OnPropertyChanged(nameof(THANHTIEN));
                }
            }
        }
        public decimal THANHTIEN
        {
            get { return GIA * SoLuong; }
        }
        public string ImagePath
        {
            get
            {
                string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                string path = "/MonAn/AnhMonAn/" + ANH;
                return path;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

        }

