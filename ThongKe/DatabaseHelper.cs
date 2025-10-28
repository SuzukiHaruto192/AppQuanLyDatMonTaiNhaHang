using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThongKe
{
    public class DatabaseHelper
    {
        
        private readonly string connectionString;

        public DatabaseHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        }
        public List<MonthlyRevenue> GetMonthlyRevenue(int year)
        {
            List<MonthlyRevenue> revenueList = new List<MonthlyRevenue>();
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        MONTH(NGAYTL) AS Thang, 
                        COUNT(MAHD) AS SoHoaDon, 
                        SUM(THANHTIEN) AS TongDoanhThu
                    FROM 
                        HoaDon
                    WHERE 
                        YEAR(NGAYTL) = @Year
                    GROUP BY 
                        MONTH(NGAYTL)
                    ORDER BY 
                        Thang";
                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Year", year);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MonthlyRevenue doanhThu = new MonthlyRevenue
                            {
                                Thang = reader.GetInt32(0),
                                SoHoaDon = reader.GetInt32(1),
                                TongDoanhThu = reader.GetDecimal(2)
                            };
                            revenueList.Add(doanhThu);
                        }
                    }
                }
            }
            return revenueList;
        }
        public List<int> GetAvailableYears()
        {
            List<int> years = new List<int>();
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT DISTINCT YEAR(NGAYTL) AS Nam
                    FROM HoaDon
                    ORDER BY Nam";
                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            years.Add(reader.GetInt32(0));
                        }
                    }
                }
            }
            return years;
        }
        public List<DaylyRevenueByCategory> GetDaylyRevenueByCategories(DateTime date)
        {
            List<DaylyRevenueByCategory> revenueList = new List<DaylyRevenueByCategory>();
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT
                        ctgr.CATEGORYNAME AS LoaiMon,
                        SUM(ct.THANHTIEN) AS DoanhThu
                    FROM HoaDon hd
                         join CTHD ct ON hd.MAHD = ct.MAHD
                         join MonAn ma ON ct.MAMON = ma.MAMON
                         join Category ctgr ON ma.CATEGORYID = ctgr.CATEGORYID
                    WHERE CAST(hd.NGAYTL AS DATE) = @Date
                    GROUP BY ctgr.CATEGORYNAME";
                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", date.Date);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DaylyRevenueByCategory doanhThu = new DaylyRevenueByCategory
                            {
                                TenCategory = reader.GetString(0),
                                TongDoanhThu = reader.GetDecimal(1)
                            };
                            revenueList.Add(doanhThu);
                        }
                    }
                }
            }
            return revenueList;
        }
        public DaylyRevenue GetDaylyRevenue(DateTime date)
        {
            DaylyRevenue revenue = null;
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        SUM(THANHTIEN) AS TongDoanhThu, 
                        COUNT(MAHD) AS SoHoaDon
                    FROM 
                        HoaDon
                    WHERE 
                        CAST(NGAYTL AS DATE) = @Date";
                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", date.Date);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            revenue = new DaylyRevenue
                            {
                                TongDoanhThu = reader.IsDBNull(0) ? 0 : reader.GetDecimal(0),
                                SoHoaDon = reader.IsDBNull(1) ? 0 : reader.GetInt32(1)
                            };
                        }
                    }
                }
            }
            return revenue;
        }
        public List<BestSellerItem> GetBestSellerItems( DateTime date) 
        { 
            List<BestSellerItem> bestSellers = new List<BestSellerItem>();
            using (var connection = new System.Data.SqlClient.SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT TOP 3
                        ma.TENMON AS TenSanPham,
                        SUM(ct.SOLUONG) AS SoLuongBan,
                        SUM(ct.THANHTIEN) AS TongDoanhThu
                    FROM HoaDon hd
                        JOIN CTHD ct ON hd.MAHD = ct.MAHD
                        JOIN MonAn ma ON ct.MAMON = ma.MAMON
                    WHERE CAST(hd.NGAYTL AS DATE) = @Date
                    GROUP BY 
                        ma.TENMON
                    ORDER BY 
                        SoLuongBan DESC";

                using (var command = new System.Data.SqlClient.SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Date", date.Date);
                    using (var reader = command.ExecuteReader())
                    {   
                        while (reader.Read())
                        {
                            BestSellerItem item = new BestSellerItem
                            {
                                TenMon = reader.GetString(0),
                                SoLuong = reader.GetInt32(1),
                                TongDoanhThu = reader.GetDecimal(2).ToString("N0")
                            };
                            bestSellers.Add(item);
                        }
                    }
                }
            }
            return bestSellers;
        }
    }
}