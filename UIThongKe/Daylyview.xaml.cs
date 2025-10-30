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

namespace UIThongKe
{
    /// <summary>
    /// Interaction logic for Daylyview.xaml
    /// </summary>
    /// 
   
    public partial class Daylyview : UserControl
    {
        public SeriesCollection MyPieChartCollection { get; set; }
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
            tbSelectDay.Text= $"{date.Day} tháng {date.Month} {date.Year}";
        }
        public Daylyview()
        {
            InitializeComponent();
            GetDaylyRevenue(DateTime.Now);
            DataContext = this;
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                DateTime selectedDate = (DateTime)e.AddedItems[0];
                GetDaylyRevenue(selectedDate);
                DataContext = null;
                DataContext = this;
            }
        }
    }
}
