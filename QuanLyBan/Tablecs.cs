using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyBan
{
    public enum TableStatus
    {
        Available,
        Occupied,
        Reserved
    }

    public class Table : INotifyPropertyChanged
    {
        private string _tableNumber;
        public string TableNumber
        {
            get => _tableNumber;
            set { _tableNumber = value; OnPropertyChanged(); }
        }

        private TableStatus _status;
        public TableStatus Status
        {
            get => _status;
            set { _status = value; OnPropertyChanged(); }
        }

        private int _capacity;
        public int Capacity
        {
            get => _capacity;
            set { _capacity = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
