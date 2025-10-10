using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

  public class MonAn : INotifyPropertyChanged
    {
        private string ten;
        private int gia;
        private string hinhAnh; // đường dẫn hình ảnh

        public string Ten
        {
            get => ten;
            set
            {
                if (ten != value)
                {
                    ten = value;
                    OnPropertyChanged(nameof(Ten));
                }
            }
        }

        public int Gia
        {
            get => gia;
            set
            {
                if (gia != value)
                {
                    gia = value;
                    OnPropertyChanged(nameof(Gia));
                }
            }
        }

        public string HinhAnh
        {
            get => hinhAnh;
            set
            {
                if (hinhAnh != value)
                {
                    hinhAnh = value;
                    OnPropertyChanged(nameof(HinhAnh));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

