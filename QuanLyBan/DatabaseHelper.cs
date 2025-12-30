using Microsoft.Data.SqlClient;
using QuanLyBan.Ban;
using QuanLyBan.HoaDon;
using QuanLyBan.Menu;
using QuanLyBan.ThongKe;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace QuanLyBan
{
    class DatabaseHelper
    {
        private readonly string connectionString;

        public DatabaseHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        }

        public List<string> GetListCategory()
        {
            List<string> list = new List<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT C.CATEGORYNAME
                    FROM Category C
                    WHERE C.CATEGORYID > 'L11'";

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader["CATEGORYNAME"].ToString());
                        }
                    }
                }
            }
            return list;
        }

        public void ThemMonAn(MonAn monAn)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "INSERT INTO MONAN (MAMON, TENMON, ANH, TAGTINHCHAT, TAGMUCDICH, MOTA, GIA, CATEGORYID) "
                            + "VALUES (@MaMon, @TenMon, @Anh, @TagTinhChat, @TagMucDich, @MoTa, @Gia, @CategoryID)";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@MaMon", monAn.MaMon);
                    cmd.Parameters.AddWithValue("@TenMon", monAn.Ten);
                    cmd.Parameters.AddWithValue("@Anh", monAn.HinhAnh);
                    cmd.Parameters.AddWithValue("@TagTinhChat", monAn.TagTinhChat);
                    cmd.Parameters.AddWithValue("@TagMucDich", monAn.TagMucDich);
                    cmd.Parameters.AddWithValue("@MoTa", monAn.MoTa);
                    cmd.Parameters.AddWithValue("@Gia", monAn.Gia);
                    cmd.Parameters.AddWithValue("@CategoryID", monAn.Category);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void XoaMon(string MaMon)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"DELETE FROM MonAn WHERE MaMon = @MaMon";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@MaMon", MaMon);

                    cmd.ExecuteNonQuery();
                }
            }

        }
        public List<MonAn> GetListMonAn()
        {
            List<MonAn> list = new List<MonAn>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = @"
                                SELECT DISTINCT
                                    m.MAMON, 
                                    m.TENMON, 
                                    m.ANH, 
                                    m.MOTA, 
                                    m.GIA, 
                                    c.CATEGORYNAME
                                FROM MonAn m
                                LEFT JOIN Category c ON m.CATEGORYID = c.CATEGORYID";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            MonAn mon = new MonAn();

                            mon.MaMon = reader["MAMON"].ToString();
                            mon.Ten = reader["TENMON"].ToString();
                            mon.HinhAnh = reader["ANH"].ToString();
                            mon.MoTa = reader["MOTA"].ToString();
                            mon.Gia = Convert.ToInt32(reader["GIA"]);
                            mon.Category = reader["CATEGORYNAME"].ToString();

                            list.Add(mon);
                        }
                    }
                }
                return list;
            }
        }

        public List<Table> GetListTables()
        {
            List<Table> tables = new List<Table>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        *
                    FROM Ban";
                using (var command = new SqlCommand(query, connection))
                {
                    using (var reader = command.ExecuteReader())
                    {
                        string status = "";
                        while (reader.Read())
                        {
                            status = reader["TrangThai"].ToString();
                            TableStatus tableStatus = new TableStatus();
                            if (status == "Trống") tableStatus = TableStatus.Available;
                            else if (status=="Đang phục vụ") tableStatus = TableStatus.Occupied;
                            else if (status=="Đã đặt trước") tableStatus = TableStatus.Reserved;
                            Table doanhThu = new Table
                            {
                                TableNumber = reader["MABAN"].ToString(),
                                Capacity = reader.GetInt32(reader.GetOrdinal("SUCCHUA")),
                                Status = tableStatus
                            };
                            tables.Add(doanhThu);
                        }
                    }
                }
            }
            return tables ;
        }
        public List<OrderItem> GetHoaDon(string MABAN)
        {
            List <OrderItem> hoaDon = new List<OrderItem>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT MA.TENMON, CT.SOLUONG, MA.GIA
                    FROM HoaDon HD
                    JOIN CTHD CT
                    ON HD.MAHD = CT.MAHD
                    JOIN MonAn MA
                    ON CT.MAMON = MA.MAMON
                    WHERE HD.MABAN = @MABAN AND HD.TRANGTHAI <> N'Đã Thanh Toán'
                    ";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MABAN", MABAN);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            OrderItem Item = new OrderItem();
                            Item.ItemName = reader["TENMON"].ToString();
                            Item.Quantity = reader.GetInt32(1);
                            Item.Price = reader.GetDecimal(2);
                            hoaDon.Add(Item);
                        }
                    }
                }
            }
            return hoaDon;
        }

        public string GetMaMon()
        {
            string mamon = "";
            using ( SqlConnection connection = new SqlConnection(connectionString) )
            {
                connection.Open();
                string query = "SELECT TOP 1 MAMON " +
                                "FROM MonAn " +
                                "ORDER BY MAMON DESC";
                using ( SqlCommand cmd = new SqlCommand(query, connection))
                {
                    using ( var reader =  cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            mamon = reader["MAMON"].ToString();
                    }
                }
            }
            if (mamon == "")
                return "M001";
            return mamon;
        }
        public string GetTrangThaiHoaDon(string maHD)
        {
            string trangthai = "";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT TRANGTHAI " +
                                "FROM HoaDon " +
                                "WHERE MAHD = @mahd";
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@mahd", maHD);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                            trangthai = reader["TRANGTHAI"].ToString();
                    }
                }
            }
            return trangthai;
        }
        public Invoice getIteamHoaDon(string MABAN)
        {
            Invoice hd = new Invoice();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT HD.MAHD, HD.GIOVAO, HD.GIAMGIA
                    FROM HoaDon HD
                    JOIN CTHD CT
                    ON HD.MAHD = CT.MAHD
                    WHERE HD.MABAN = @MABAN AND HD.TRANGTHAI <> N'Đã Thanh Toán'
                    ";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MABAN", MABAN);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            hd.MaHD = reader["MAHD"].ToString();
                            //hd.GioVao = reader.GetDateTime(1);
                            hd.GiamGia = reader.GetDecimal(2);
                        }
                    }
                }
            }
            return hd;
        }

        public void Update_Status_Table(string MaBan, string TrangThai)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    UPDATE Ban
                    SET TRANGTHAI = @TT
                    WHERE MABAN = @MaBan
                    ";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MABAN", MaBan);
                    command.Parameters.AddWithValue("@TT", TrangThai);
                    command.ExecuteNonQuery();
                }
            }
        }



        public void Update_Status_Order(string MaHD)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    UPDATE HoaDon
                    SET TRANGTHAI = N'Đã Thanh Toán'
                    WHERE MAHD = @MaHD
                    ";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@MaHD", MaHD);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update_Total_HoaDon(string maHD, decimal subtotal, decimal grandtotal)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    UPDATE HoaDon
                    SET TAMTINH = @subtotal, THANHTIEN = @grandtotal
                    WHERE MAHD = @maHD
                ";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@maHD", maHD);
                    command.Parameters.AddWithValue("@subtotal", subtotal);
                    command.Parameters.AddWithValue("@grandtotal", grandtotal);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void KhachDi(string MaBan, string TrangThai)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Ban " +
                               "SET TRANGTHAI = @TrangThai " +
                               "WHERE MABAN = @Ma";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.AddWithValue("@TrangThai",TrangThai);
                    command.Parameters.AddWithValue("@Ma", MaBan);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateHoaDon(string MaHD, string TrangThai) // VT CÓ CHỈNH SỬA
        {
            DateTime NgayTL = DateTime.Today;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE HoaDon " +
                               "SET TRANGTHAI = @tt " +
                               "WHERE MAHD = @mahd";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {                
                    cmd.Parameters.AddWithValue("@mahd", MaHD);
                    cmd.Parameters.AddWithValue("@tt", TrangThai);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public void Delete_Item_CTHD(string TenMon)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                     DELETE FROM CTHD
                     WHERE MAMON IN (
                               SELECT MAMON
                               FROM MONAN
                               WHERE TENMON = @TenMon
                          )
                    ";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@TenMon", TenMon);
                    command.ExecuteNonQuery();
                }
            }
        }


        public List<MonthlyRevenue> GetMonthlyRevenue(int year)
        {
            List<MonthlyRevenue> revenueList = new List<MonthlyRevenue>();
            using (var connection = new SqlConnection(connectionString))
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
                      AND TRANGTHAI = N'Đã thanh toán'
                    GROUP BY 
                        MONTH(NGAYTL)
                    ORDER BY 
                        Thang";
                using (var command = new SqlCommand(query, connection))
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
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT DISTINCT YEAR(NGAYTL) AS Nam
                    FROM HoaDon
                    ORDER BY Nam";
                using (var command = new SqlCommand(query, connection))
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
            using (var connection = new SqlConnection(connectionString))
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
                      AND hd.TRANGTHAI = N'Đã thanh toán'
                    GROUP BY ctgr.CATEGORYNAME";
                using (var command = new SqlCommand(query, connection))
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
            DaylyRevenue revenue = new DaylyRevenue();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        SUM(THANHTIEN) AS TongDoanhThu, 
                        COUNT(MAHD) AS SoHoaDon
                    FROM 
                        HoaDon
                    WHERE 
                        CAST(NGAYTL AS DATE) = @Date
                    AND TRANGTHAI = N'Đã thanh toán'";
                using (var command = new SqlCommand(query, connection))
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
        public List<FoodRevenue> GetTopFoodRevenues(int month, int year, int topN)
        {
            List<FoodRevenue> foodRevenues = new List<FoodRevenue>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT TOP(@TopN)
                        ma.TENMON AS TenMon,
                        SUM(ct.THANHTIEN) AS DoanhThu
                    FROM HoaDon hd
                         JOIN CTHD ct ON hd.MAHD = ct.MAHD
                         JOIN MonAn ma ON ct.MAMON = ma.MAMON
                    WHERE MONTH(hd.NGAYTL)= @Month and YEAR(hd.NGAYTL) = @Year
                      AND hd.TRANGTHAI = N'Đã thanh toán'
                    GROUP BY ma.TENMON
                    ORDER BY DoanhThu DESC";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Month", month);
                    command.Parameters.AddWithValue("@Year", year);
                    command.Parameters.AddWithValue("@TopN", topN);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            FoodRevenue foodRevenue = new FoodRevenue
                            {
                                TenMon = reader.GetString(0),
                                DoanhThu = reader.GetDecimal(1)
                            };
                            foodRevenues.Add(foodRevenue);
                        }
                    }
                }
            }
            return foodRevenues;
        }
        public List<DaylyRevenue> GetDayRevenueForMonthlyView(int month, int year)
        {
            List<DaylyRevenue> revenueList = new List<DaylyRevenue>();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string query = @"
                    SELECT 
                        DAY(NGAYTL) AS Ngay, 
                        SUM(THANHTIEN) AS TongDoanhThu, 
                        COUNT(MAHD) AS SoHoaDon
                    FROM 
                        HoaDon
                    WHERE 
                        MONTH(NGAYTL) = @Month AND YEAR(NGAYTL) = @Year
                     AND TRANGTHAI = N'Đã thanh toán'
                    GROUP BY 
                        DAY(NGAYTL)
                    ORDER BY 
                        Ngay";
                using (var command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Month", month);
                    command.Parameters.AddWithValue("@Year", year);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            DaylyRevenue doanhThu = new DaylyRevenue
                            {
                                Ngay = reader.GetInt32(0),
                                TongDoanhThu = reader.GetDecimal(1),
                                SoHoaDon = reader.GetInt32(2)
                            };
                            revenueList.Add(doanhThu);
                        }
                    }
                }
            }
            return revenueList;
        }

        public string GetDuongDanAnh(MonAn monAn)
        {
            string path = "";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT ANH FROM MonAn WHERE MAMON = @MaMon";
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    cmd.Parameters.AddWithValue("@MaMon", monAn.MaMon);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while( reader.Read())
                        {
                            path = reader["ANH"].ToString();
                        }    
                    }
                }
            }
            return path;
        }
    }
}
