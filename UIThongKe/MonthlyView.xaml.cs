using LiveCharts;
using LiveCharts.Wpf;
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

namespace UIThongKe
{
    /// <summary>
    /// Interaction logic for MonthlyView.xaml
    /// </summary>
    public partial class MonthlyView : UserControl
    {
        public SeriesCollection MySeriesCollection { get; set; }
        public string[] MyLabels { get; set; }
        public string TitleChart { get; set; }
        public Func<double, string> MyFormatterRevenue { get; set; }
        public Func<double, string> MyFormatterOrders { get; set; }
        public List<int> ListYears { get; set; } = new DatabaseHelper().GetAvailableYears();
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
                Fill = new SolidColorBrush(Color.FromRgb(93, 100, 203)),
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
            tbYear.Text = "Năm "+year.ToString();
        }
        public SeriesCollection MySeriesCollectionTopRevenue { get; set; }
        public SeriesCollection MySeriesCollectionDetailsDaylyRevenue { get; set; }
        public string[] MyLabelsDetailsDaylyRevenue { get; set; }
        public void GetTopFoodRevenue(int month, int year, int topN)
        {
            MySeriesCollectionTopRevenue = new SeriesCollection();
            List<FoodRevenue> toprevenue= new DatabaseHelper().GetTopFoodRevenues(month, year, topN);
            foreach (var food in toprevenue)
            {
                MySeriesCollectionTopRevenue.Add(new RowSeries
                {
                    Title = food.TenMon,
                    Values = new ChartValues<double> { (double)food.DoanhThu },
                    DataLabels = true,
                    MaxRowHeigth = 50,
                    LabelPoint = point => food.TenMon
                });
            }
            List<DaylyRevenue> daylyRevenues = new DatabaseHelper().GetDayRevenueForMonthlyView(month, year);
          
                MySeriesCollectionDetailsDaylyRevenue = new SeriesCollection
                {
                    new LiveCharts.Wpf.LineSeries
                    {
                        Title = "Doanh Thu",
                        Values = new LiveCharts.ChartValues<double>(
                            daylyRevenues.Select(r => (double)r.TongDoanhThu)),
                    },
                    new LiveCharts.Wpf.LineSeries
                    {
                        Title = "Số Hóa Đơn",
                        Values = new LiveCharts.ChartValues<double>(
                            daylyRevenues.Select(r => (double)r.SoHoaDon)),
                        ScalesYAt = 1
                    }
                };
            MyLabelsDetailsDaylyRevenue= daylyRevenues.Select(r =>r.Ngay.ToString()).ToArray();
            MyFormatterRevenue = value => value.ToString("N0");
            MyFormatterOrders = value => ((int)value).ToString();
            tbDetailsMonthly.Text = $"Chi tiết tháng {month}";
            tbTopRevenue.Text = $"Top doanh thu tháng {month}";
            DataContext = null;
            DataContext = this;
        }

        public MonthlyView()
        {
            InitializeComponent();
            GetMonthlyRevenue(DateTime.Now.Year);
            GetTopFoodRevenue(DateTime.Now.Month,DateTime.Now.Year,10);
            DataContext = this;
        }

        private void cbbSelectYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbbSelectYear.SelectedItem != null)
            {
                int selectedYear = (int)cbbSelectYear.SelectedItem;
                GetMonthlyRevenue(selectedYear);
                GetTopFoodRevenue(Convert.ToInt32(MyLabels[MyLabels.Length-1].Substring(6)), selectedYear, 10);
                DataContext = null;
                DataContext = this;
            }
        }

        private void chartRevenueOfYear_DataClick(object sender, ChartPoint chartPoint)
        {
            GetTopFoodRevenue(Convert.ToInt32(MyLabels[(int)chartPoint.X].Substring(6)),Convert.ToInt32(tbYear.Text.Substring(4)),10);
        }
    }
}
