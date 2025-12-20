using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Configuration;

namespace QuanLyBan.Ban
{
    public class TableStatusMonitor
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        private readonly Dispatcher _dispatcher;

        // Sự kiện để thông báo cho ViewModel Quản lý Bàn
        public event Action OnTableStatusChanged;

        // Constructor: nhận Dispatcher của UI Thread để cập nhật giao diện an toàn
        public TableStatusMonitor(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void StartMonitor()
        {
            try
            {
                // 1. Khởi động SqlDependency (Chỉ cần gọi 1 lần khi ứng dụng mở)
                SqlDependency.Start(connectionString);

                // 2. Bắt đầu đăng ký truy vấn lần đầu
                MonitorTableStatus();
            }
            catch (Exception ex)
            {
                // Xử lý lỗi (ví dụ: Service Broker chưa bật)
                Console.WriteLine("Lỗi khởi động SqlDependency: " + ex.Message);
            }
        }

        public void StopMonitor()
        {
            // Dừng lắng nghe khi ứng dụng đóng
            SqlDependency.Stop(connectionString);
        }

        // Hàm cốt lõi: Đăng ký truy vấn và thiết lập lắng nghe
        private void MonitorTableStatus()
        {
            // Đảm bảo câu truy vấn tuân thủ quy tắc: PHẢI chỉ định rõ cột, không dùng SELECT *
            string query = "SELECT MABAN, TRANGTHAI FROM [dbo].[Ban]";

            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand(query, connection))
            {
                // 3. Tạo SqlDependency MỚI cho mỗi lần đăng ký
                SqlDependency dependency = new SqlDependency(command);

                // 4. Đăng ký sự kiện
                dependency.OnChange += Dependency_OnChange;

                connection.Open();

                // 5. Thực thi truy vấn để kích hoạt việc đăng ký trên Server
                command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        // Hàm Xử lý sự kiện khi có thay đổi
        private void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            // *BƯỚC BẮT BUỘC*: Hủy đăng ký sự kiện cũ
            SqlDependency dependency = sender as SqlDependency;
            if (dependency != null)
            {
                dependency.OnChange -= Dependency_OnChange;
            }

            if (e.Type == SqlNotificationType.Change)
            {
                // Tái đăng ký lắng nghe ngay lập tức cho lần thay đổi tiếp theo
                MonitorTableStatus();

                // Chuyển việc cập nhật giao diện sang UI Thread
                _dispatcher.Invoke(() =>
                {
                    // Thông báo cho ViewModel rằng dữ liệu bàn đã thay đổi
                    OnTableStatusChanged?.Invoke();
                });
            }
        }
    }
}

