using doanlttq.ChonVoucher;
using doanlttq.Services;
using doanlttq.ViewModels;
using Microsoft.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Windows;

namespace doanlttq
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string MaHoaDon {  get; set; }
        public DateTime GioVao { get; set; }
        public DateTime GioRa { get; set; }
        public string MABAN {  get; set; }
        public bool ThemHDFirst { get; set; }
        public decimal TongTien { get; set; }
        // TÍCH ĐIỂM
        public string MaKH { get; set; }
        public Voucher? VoucherApDung { get; set; }
        // END TÍCH ĐIỂM
        private string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;

        public OrderViewModel _sharedViewModel;

        // 1. HÀM CHẠY KHI PHẦN MỀM BẮT ĐẦU (START)
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // 1. KHỞI TẠO CÁC SERVICE
            var iCarsService = new ICARS_ScoringService();
            var weatherService = new WeatherService();


            _sharedViewModel = new OrderViewModel(iCarsService, weatherService);

            try
            {
                // Bắt đầu lắng nghe thay đổi từ SQL
                SqlDependency.Start(connectionString);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khởi động SQL Service: " + ex.Message);
            }
        }

        // 2. HÀM CHẠY KHI TẮT PHẦN MỀM (EXIT)
        protected override void OnExit(ExitEventArgs e)
        {
            try
            {
                // Dừng lắng nghe để giải phóng tài nguyên
                SqlDependency.Stop(connectionString);
            }
            catch (Exception)
            {
                // Bỏ qua lỗi khi tắt app
            }
            base.OnExit(e);
        }
    }

}
