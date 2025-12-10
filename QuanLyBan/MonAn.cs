using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBan
{
    public class MonAn : INotifyPropertyChanged
    {
        private string maMon;
        private string ten;
        private int gia;
        private string hinhAnh;
        private string moTa;
        private string tagTinhChat;
        private string tagMucDich;
        private string category;

        public string MaMon
        {
            get => maMon;
            set
            {
                if (maMon != value)
                {
                    maMon = value;
                    OnPropertyChanged(nameof(MaMon));
                }
            }
        }
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
        public string MoTa
        {
            get => moTa;
            set
            {
                if (moTa != value)
                {
                    moTa = value;
                    OnPropertyChanged(nameof(MoTa));
                }
            }
        }
        public string Category
        {
            get => category;
            set
            {
                if (category != value)
                {
                    category = value;
                    OnPropertyChanged(nameof(Category));
                }
            }
        }
        public string TagTinhChat
        {
            get => tagTinhChat;
            set
            {
                if (tagTinhChat != value)
                {
                    tagTinhChat = value;
                    OnPropertyChanged(nameof(TagTinhChat));
                }
            }
        }
        public string TagMucDich
        {
            get => tagMucDich;
            set
            {
                if (tagMucDich != value)
                {
                    tagMucDich = value;
                    OnPropertyChanged(nameof(TagMucDich));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

