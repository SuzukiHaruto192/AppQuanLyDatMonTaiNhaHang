using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace QuanLyBan
{
    public class TableManagementViewModel : INotifyPropertyChanged
    {
        private static readonly Random _random = new Random();  //Bien de tao random (bo di khi ket noi database)
        private string _currentFilter = "Tất cả bàn";
        public ObservableCollection<Table> AllTables { get; set; }
        public ICollectionView Tables { get; }

        private Table? _selectedTable;
        public Table? SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                OnPropertyChanged();
            }
        }
        public event Action<Table>? RequestShowInvoice;

        //Danh sach command
        public ICommand BookTableCommand { get; }
        public ICommand ClearTableCommand { get; }
        public ICommand FilterTableCommand { get; }
        //Ket thuc danh sach command

        public TableManagementViewModel()
        {
            AllTables = new ObservableCollection<Table>();
            LoadTables();

            BookTableCommand = new RelayCommand(BookTable, CanBookTable);
            ClearTableCommand = new RelayCommand(ShowInvoice, CanClearTable);
            FilterTableCommand = new RelayCommand(Filter);

            Tables = CollectionViewSource.GetDefaultView(AllTables);
            Tables.Filter = FilterTablesPredicate;
        }

        private void BookTable(object? parameter)
        {
            if (SelectedTable != null)
            {
                SelectedTable.Status = TableStatus.Occupied;
            }
        }
        private bool CanBookTable(object? parameter)
        {
            return SelectedTable != null && SelectedTable.Status == TableStatus.Available;
        }

        private void ShowInvoice(object? parameter)
        {
            if (SelectedTable != null)
            {
                RequestShowInvoice?.Invoke(SelectedTable);
            }
        }
        private bool CanClearTable(object? parameter)
        {
            return SelectedTable != null && SelectedTable.Status != TableStatus.Available;
        }
        private void Filter(object? parameter)
        {
            _currentFilter = parameter as string;
            SelectedTable = null;
            Tables.Refresh();
        }
        private bool FilterTablesPredicate(object obj)
        {
            if (obj is Table table)
            {
                switch (_currentFilter)
                {
                    case "Bàn trống":
                        return table.Status == TableStatus.Available;
                    case "Đang phục vụ":
                        return table.Status == TableStatus.Occupied;
                    case "Đã đặt trước":
                        return table.Status == TableStatus.Reserved;
                    case "Tất cả":
                    default:
                        return true;
                }
            }
            return false;
        }
        public void FinalizePayment(Table table)
        {
            if (table != null)
            {
                table.Status = TableStatus.Available;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //Tạo dữ liệu mẫu ngẫu nhiên
        private void LoadTables()
        {
            for (int i = 5; i <= 20; i++)
            {
                AllTables.Add(new Table
                {
                    TableNumber = i,
                    Status = GetRandomStatus(),
                    Capacity = (_random.Next(3) + 1) * 2
                });
            }
        }
        private TableStatus GetRandomStatus()
        {
            var values = System.Enum.GetValues(typeof(TableStatus));
            return (TableStatus)values.GetValue(_random.Next(values.Length));
        }
    }
}

