using LiveCharts;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Data.SqlClient;

namespace ThongKe
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {       
        public string RevenueOfDay { get; set; }
        public string OrdersOfDay { get; set; }
        public void GetDaylyRevenue(DateTime date)
        {
            DatabaseHelper databaseHelper = new DatabaseHelper();
            DaylyRevenue revenueData = databaseHelper.GetDaylyRevenue(date);
            RevenueOfDay = "Tổng: "+revenueData.TongDoanhThu.ToString("N0")+" VND";
            OrdersOfDay ="Số đơn: "+revenueData.SoHoaDon.ToString();
        }
        public SeriesCollection MyPieChartCollection { get; set; }
        public void GetDaylyRevenueByCategory(DateTime date)
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
        }
       public void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                DateTime selectedDate = (DateTime)e.AddedItems[0];
                this.GetDaylyRevenue(selectedDate);
                this.GetDaylyRevenueByCategory(selectedDate);
                this.GetBestSellerItems(selectedDate);
                DataContext = null;
                DataContext = this;
            }
        }
        public List<BestSellerItem> BestSellerItems { get; set; }
        public void GetBestSellerItems(DateTime date)
        {
            DatabaseHelper databaseHelper = new DatabaseHelper();
            List<BestSellerItem> bestSellerData = databaseHelper.GetBestSellerItems(date);
            BestSellerItems = bestSellerData;
        }
        public SeriesCollection MySeriesCollection { get; set; }
        public string[] MyLabels { get; set; }
        public string TitleChart { get; set; }
        public Func<double, string> MyFormatterRevenue { get; set; }
        public Func<double, string> MyFormatterOrders { get; set; }
        public void GetMonthlyRevenue(int year)
        {
            DatabaseHelper databaseHelper = new DatabaseHelper();
            List<MonthlyRevenue> revenueData = databaseHelper.GetMonthlyRevenue(year);

            MySeriesCollection = new SeriesCollection
        {
            new LiveCharts.Wpf.ColumnSeries
            {
                Title = "Doanh Thu",
                Values = new LiveCharts.ChartValues<double>(
                    revenueData.Select(r => (double)r.TongDoanhThu)),
                ScalesYAt = 0
            },
            new LiveCharts.Wpf.LineSeries
            {
                Title = "Số Hóa Đơn",
                Values = new LiveCharts.ChartValues<double>(
                    revenueData.Select(r => (double)r.SoHoaDon)),
                ScalesYAt = 1
            }
        };
            TitleChart = $"Thống Kê Doanh Thu Năm {year}";
            MyLabels = revenueData.Select(r => $"Tháng {r.Thang}").ToArray();
            MyFormatterRevenue = value => value.ToString("N0");
            MyFormatterOrders = value => ((int)value).ToString();
            cbxYear.Text = year.ToString();
        }
        public List<int> Years { get; set; } 
       
        public void cbxYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                int SelectedYear = (int)e.AddedItems[0];
                this.GetMonthlyRevenue(SelectedYear);
                DataContext = null;
                DataContext = this;
            }
        }
        public void GetAvailableYears()
        {
            DatabaseHelper databaseHelper = new DatabaseHelper();
            Years = databaseHelper.GetAvailableYears();
        }
        public MainWindow()
        {
            InitializeComponent();
            this.GetDaylyRevenue(DateTime.Now);
            this.GetDaylyRevenueByCategory(DateTime.Now);
            this.GetBestSellerItems(DateTime.Now);
            this.GetMonthlyRevenue(DateTime.Now.Year);
            this.GetAvailableYears();
            DataContext = this;
        }
    }
}
