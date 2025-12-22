using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace QuanLyBan.HoaDon
{
    public class InvoiceMonitor
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        private Dispatcher _dispatcher;
        public event Action OnInvoiceChanged;

        public InvoiceMonitor(Dispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public void StartMonitor()
        {
            SqlDependency.Start(connectionString);
            MonitorInvoice();
        }

        private void StopMonitor()
        {
            SqlDependency.Stop(connectionString);
        }

        public void MonitorInvoice()
        {
            string query = "SELECT MAHD FROM [dbo].[HoaDon] WHERE TRANGTHAI = N'Đã thanh toán'";

            using ( var connection = new SqlConnection(connectionString)) 
            using (var command = new SqlCommand(query, connection))
            {
                SqlDependency sqlDependency = new SqlDependency(command);
                sqlDependency.OnChange += Dependency_OnChange;

                connection.Open();
                command.ExecuteReader(CommandBehavior.CloseConnection);
            }
        }

        public void Dependency_OnChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDependency dependency = sender as SqlDependency;

            if ( dependency != null) 
                dependency.OnChange -= Dependency_OnChange;

            if ( e.Type == SqlNotificationType.Change)
            {
                MonitorInvoice();

                _dispatcher.Invoke(() =>
                {
                    OnInvoiceChanged?.Invoke();
                });
            }    
        }
    }
}
