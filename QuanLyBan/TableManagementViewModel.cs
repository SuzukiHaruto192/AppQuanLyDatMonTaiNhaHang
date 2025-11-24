using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Windows.Data;
using System.Windows.Input;

namespace QuanLyBan
{
    public class TableManagementViewModel : INotifyPropertyChanged
    {
        //private static readonly Random _random = new Random();  //Bien de tao random (bo di khi ket noi database)
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
        public ICommand ServeTableCommand { get; }
        public ICommand ClearTableCommand { get; }
        public ICommand BookTableCommand { get; }
        public ICommand FilterTableCommand { get; }
        //Ket thuc danh sach command

        public TableManagementViewModel()
        {
            AllTables = new ObservableCollection<Table>();
            LoadTables();

            ServeTableCommand = new RelayCommand(ServeTable, CanServeTable);
            BookTableCommand = new RelayCommand(BookTable, CanBookTable);
            ClearTableCommand = new RelayCommand(ShowInvoice, CanClearTable);
            FilterTableCommand = new RelayCommand(Filter);

            Tables = CollectionViewSource.GetDefaultView(AllTables);
            Tables.Filter = FilterTablesPredicate;
        }

        private void ServeTable(object? parameter)
        {
            if (SelectedTable != null)
            {
                SelectedTable.Status = TableStatus.Occupied;
            }
        }
        private bool CanServeTable(object? parameter)
        {
            return SelectedTable != null && (SelectedTable.Status == TableStatus.Available || SelectedTable.Status == TableStatus.Reserved);
        }
        private void BookTable(object? parameter)
        {
            if (SelectedTable != null)
            {
                SelectedTable.Status = TableStatus.Reserved;
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
                DatabaseHelper db = new DatabaseHelper();
                table.Status = TableStatus.Available;
                db.Update_Status_Table(table.TableNumber);
                db.Update_Status_Order(db.getIteamHoaDon(table.TableNumber).MaHD);

            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void LoadTables()
        {
            DatabaseHelper databaseHelper = new DatabaseHelper();
            List<Table> ListTable= databaseHelper.GetListTables();
            foreach(var table in ListTable) 
            { 
                AllTables.Add(new Table
                {
                    TableNumber = table.TableNumber,
                    Status = table.Status,
                    Capacity = table.Capacity
                });
            }
        }
    }
}

