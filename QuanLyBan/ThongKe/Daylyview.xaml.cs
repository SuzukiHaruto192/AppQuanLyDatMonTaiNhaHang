using LiveCharts;
using QuanLyBan.HoaDon;
using QuanLyBan.ThongKe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace QuanLyBan
{
    public partial class Daylyview : UserControl, INotifyPropertyChanged
    {
        private SeriesCollection _myPieChart;
        public SeriesCollection MyPieChartCollection
        {
            get
            {
                return _myPieChart;
            }
            set
            {
                if (_myPieChart != value)
                {
                    _myPieChart = value;
                    OnPropertyChanged();
                }
            }
        }

        private InvoiceMonitor _monitor;
        
        public Daylyview()
        {
            InitializeComponent();
           
            Dispatcher dispatcher = Application.Current.Dispatcher;
            _monitor = new InvoiceMonitor(dispatcher);
            _monitor.StartMonitor();
            _monitor.OnInvoiceChanged += Reload;

            GetDaylyRevenue(DateTime.Now);
            DataContext = this;

        }

        public void Reload()
        {
            DateTime ngay = DateTime.Now;
            GetDaylyRevenue(ngay);
        }

        public void GetDaylyRevenue(DateTime date)
        {
            DatabaseHelper databaseHelper = new DatabaseHelper();
            List<DaylyRevenueByCategory> revenueData = databaseHelper.GetDaylyRevenueByCategories(date);
            MyPieChartCollection = new SeriesCollection();
            foreach (var item in revenueData)
            {
                MyPieChartCollection.Add(new LiveCharts.Wpf.PieSeries
                {
                    Title = item.TenCategory,
                    Values = new LiveCharts.ChartValues<decimal> { item.TongDoanhThu },
                    DataLabels = false,
                    LabelPoint = chartPoint => string.Format("{0} ({1:P})", item.TongDoanhThu.ToString("N0"), chartPoint.Participation)
                });
            }
            DaylyRevenue revenue = databaseHelper.GetDaylyRevenue(date);
            tbRevenue.Text = revenue.TongDoanhThu.ToString("N0") + " VND";
            tbOrder.Text = revenue.SoHoaDon.ToString();
            tbSelectDay.Text = $"{date.Day} tháng {date.Month} {date.Year}";
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                DateTime selectedDate = (DateTime)e.AddedItems[0];
                GetDaylyRevenue(selectedDate);
                //DataContext = null;
                //DataContext = this;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
