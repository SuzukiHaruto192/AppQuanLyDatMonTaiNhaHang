using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace QuanLyBan
{
    public class InvoiceViewModel : INotifyPropertyChanged
    {
        public Table TableToBill { get; set; }                      //Ban dang thanh toan
        public string InvoiceID { get; set; }                    //Ma hoa don
   //     public DateTime InvoiceDateIn { get; set; }                  //Thoi diem bat dau
        public DateTime InvoiceDate { get; set; }                   //Thoi diem thanh toan
        public ObservableCollection<OrderItem> Items { get; set; }  //Danh sach mon da goi
        public decimal Subtotal => Items.Sum(item => item.Total);   //So tien tam tinh

        private decimal discountAmount;                             //Luong giam gia
        public decimal DiscountAmount
        {
            get => discountAmount;
            set
            {
                discountAmount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(GrandTotal));
            }
        }
        public decimal GrandTotal => Subtotal - DiscountAmount;     //Tong tien phai thanh toan
        private bool _isEditing;                                    //Kiem tra xem co dang chinh sua khong
        public bool IsEditing
        {
            get => _isEditing;
            set
            {
                _isEditing = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(EditButtonText));
                OnPropertyChanged(nameof(IsReadOnly));
            }
        }
        public string EditButtonText => IsEditing ?  "Cập nhật" : "Chỉnh sửa" ;
        public ICommand RemoveItemCommand { get; }                  //Ham xoa mon trong hoa don
        public ICommand EditModeCommand { get; }                    //Bat tat che do chinh sua
        public InvoiceViewModel(Table tableToBill)
        {
            DatabaseHelper dp=new DatabaseHelper();
            HoaDon hd = dp.getIteamHoaDon(tableToBill.TableNumber);
            TableToBill = tableToBill;
            InvoiceID = hd.MaHD;
            //InvoiceDateIn = hd.GioVao;
            InvoiceDate = DateTime.Now;
            discountAmount = hd.GiamGia;
            IsEditing = false;

            EditModeCommand = new RelayCommand(EditMode);
            RemoveItemCommand = new RelayCommand(RemoveItem);
            List<OrderItem> listitem = dp.GetHoaDon(tableToBill.TableNumber);
            foreach (OrderItem item in listitem)
            {
                Console.WriteLine(item.ItemName+' '+item.Quantity+' '+item.Price);
            }
            Items = new ObservableCollection<OrderItem>(listitem);
            //Du lieu thu nghiem

            //Items = new ObservableCollection<OrderItem>
            //{
            //    new OrderItem { ItemName = "Gà rán", Quantity = 2, Price = 50000 },
            //    new OrderItem { ItemName = "Khoai tây chiên", Quantity = 1, Price = 30000 },
            //    new OrderItem { ItemName = "Coca-Cola (L)", Quantity = 2, Price = 15000 },
            //    new OrderItem { ItemName = "Salad Цезарь", Quantity = 1, Price = 45000 }
            //};
            //DiscountAmount = 10000;
            ////Ket thuc du lieu thu nghiem
            //Items.CollectionChanged += Items_CollectionChanged;

            foreach (OrderItem item in Items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }
        public bool IsReadOnly => !IsEditing;
        private void EditMode(object? parameter) 
        {
            IsEditing = !IsEditing;
            OnPropertyChanged(nameof(EditMode));
        }
        private void RemoveItem(object? parameter)
        {
            if (parameter is OrderItem item) 
            {
                Items.Remove(item);
            };
        }
        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Total")
            {
                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(GrandTotal));
            }
        }
        private void Items_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (INotifyPropertyChanged item in e.NewItems)
                    item.PropertyChanged += Item_PropertyChanged;

            if (e.OldItems != null)
                foreach (INotifyPropertyChanged item in e.OldItems)
                    item.PropertyChanged -= Item_PropertyChanged;
            
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(GrandTotal));
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
