using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.Arm;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;

namespace QuanLyBan.Ban
{
    public class TableManagementViewModel : INotifyPropertyChanged
    {
        private string _currentFilter = "Tất cả bàn";

        private TableStatusMonitor _monitor;
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
        private DatabaseHelper db = new DatabaseHelper();
        public TableManagementViewModel()
        {
            AllTables = new ObservableCollection<Table>();

            ServeTableCommand = new RelayCommand(ServeTable, CanServeTable);
            BookTableCommand = new RelayCommand(BookTable, CanBookTable);
            ClearTableCommand = new RelayCommand(ShowInvoice, CanClearTable);
            FilterTableCommand = new RelayCommand(Filter);

            Tables = CollectionViewSource.GetDefaultView(AllTables);
            Tables.Filter = FilterTablesPredicate;

            Dispatcher dispatcher = Application.Current.Dispatcher;

            _monitor = new TableStatusMonitor(dispatcher);
            _monitor.OnTableStatusChanged += ReloadTableStatusFromDatabase; // Gán hàm xử lý
            _monitor.StartMonitor();

            LoadTables(); // Tải dữ liệu lần đầu
        }

        private void ReloadTableStatusFromDatabase()
        {
            DatabaseHelper db = new DatabaseHelper();
            // 1. Truy vấn Database để lấy trạng thái mới nhất của TẤT CẢ các bàn
            List<Table> newStatuses = db.GetListTables();

            // 2. Cập nhật ObservableCollection<Table> hiện tại
            // Duyệt qua và cập nhật thuộc tính Status (trạng thái) của các Table hiện có
            foreach (var table in AllTables)
            {
                var newStatus = newStatuses.FirstOrDefault(t => t.TableNumber == table.TableNumber)?.Status;
                if (newStatus != null && table.Status != newStatus)
                {
                    table.Status = (TableStatus)newStatus; // Kích hoạt PropertyChanged và chuyển màu
                }
            }
        }

        private void ServeTable(object? parameter)
        {
            if (SelectedTable != null)
            {
                SelectedTable.Status = TableStatus.Occupied;
                db.Update_Status_Table(SelectedTable.TableNumber, "Đang phục vụ");
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
                db.Update_Status_Table(SelectedTable.TableNumber, "Đã đặt trước");
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
                db.Update_Status_Table(table.TableNumber, "Trống");
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
            List<Table> ListTable = db.GetListTables();
            foreach (var table in ListTable)
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

