using doanlttq.MonAn;
using Microsoft.Data.SqlClient; // ✅ Dùng Microsoft.Data.SqlClient thay vì System.Data.SqlClient
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Media;


namespace doanlttq
{


    public class DatabaseHelper
    {
        private readonly string connectionString;

        public DatabaseHelper()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyConnection"].ConnectionString;
        }

        public List<Food> GetFoods()
        {
            List<Food> foods = new List<Food>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT * FROM MonAn";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string tenFileAnh = reader["ANH"] as string ?? "";

                    string duongDanAnh = "MonAn/AnhMonAn/" + tenFileAnh;
                    foods.Add(new Food
                    {
                        MAMON = (string)reader["MAMON"],
                        TENMON = (string)reader["TENMON"],
                        GIA = (decimal)reader["GIA"],
                        ANH = duongDanAnh,
                        MOTA = (string)reader["MOTA"],
                        TRANGTHAI = (string)reader["TRANGTHAI"],
                        CATEGORYID = (string)reader["CATEGORYID"],
                        SoLuong = 1
                    });
                }
            }

            return foods;
        }
        public List<Food> LocMonAnChuDe(  string LoaiMon)
        {
            List<Food> foods = new List<Food>();
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                conn.Open();
                string query = "select ma.MAMON , ma.TENMON , ma.GIA , ma.ANH , ma.MOTA , ma.TRANGTHAI , ma.CATEGORYID  " +
                    "From MonAn ma Join Category ct on ma.CATEGORYID = ct.CATEGORYID Where ct.PARENTCATEGORYID = '"+LoaiMon+"'";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string tenFileAnh = reader["ANH"] as string ?? "";

                    string duongDanAnh = "MonAn/AnhMonAn/" + tenFileAnh;
                    foods.Add(new Food
                    {
                        MAMON = (string)reader["MAMON"],
                        TENMON = (string)reader["TENMON"],
                        GIA = (decimal)reader["GIA"],
                        ANH = duongDanAnh,
                        MOTA = (string)reader["MOTA"],
                        TRANGTHAI = (string)reader["TRANGTHAI"],
                        CATEGORYID = (string)reader["CATEGORYID"],
                        SoLuong = 1
                    });
                }

            }
            return foods ;
        }
        public List<Food> TimMon(string TenMon, string LoaiMon)
        {
            List<Food> foods = new List<Food>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "select ma.MAMON , ma.TENMON , ma.GIA , ma.ANH , ma.MOTA , ma.TRANGTHAI , ma.CATEGORYID " +
                    "From MonAn ma Join Category ct on ma.CATEGORYID = ct.CATEGORYID Where ct.PARENTCATEGORYID = '" + LoaiMon + "' and TENMON like N'%" + TenMon + "%'";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    string tenFileAnh = reader["ANH"] as string ?? "";

                    string duongDanAnh = "MonAn/AnhMonAn/" + tenFileAnh;
                    foods.Add(new Food
                    {
                        MAMON = (string)reader["MAMON"],
                        TENMON = (string)reader["TENMON"],
                        GIA = (decimal)reader["GIA"],
                        ANH = duongDanAnh,
                        MOTA = (string)reader["MOTA"],
                        TRANGTHAI = (string)reader["TRANGTHAI"],
                        CATEGORYID = (string)reader["CATEGORYID"],
                        SoLuong = 1
                    });
                }

            }
            return foods;
        }

        public int LayMaHoaDon()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "select TOP 1 MAHD FROM HOADON ORDER BY NGAYTL DESC , GIORA DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                object result = cmd.ExecuteScalar();

                if (result == DBNull.Value || result == null)
                {
                    return 0;
                }
                string maLonNhat = result.ToString(); 

                string phanSo = maLonNhat.Substring(2); 

                return Convert.ToInt32(phanSo)+1;
            }
        }
        public bool TimMAKH(string sdt)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT 1 FROM KhachHang WHERE MAKH = @makh";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@makh", sdt); 

                object result = cmd.ExecuteScalar();
                if (result == null)
                {
                    return false;
                }

                return true;
            }
        }
        public void ThemHoaDon(decimal ThanhTien, string maKhachHangSdt)
        {
            DateTime NgayTL = DateTime.Today;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO HoaDon (GIOVAO, GIORA, NGAYTL, MAHD, THANHTIEN, MAKH) " +
                               "VALUES (@gio_vao, @gio_ra, @Ngaytl, @mahd, @thanh_tien, @maKhachHang)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@gio_vao", ((App)Application.Current).GioVao);
                cmd.Parameters.AddWithValue("@gio_ra", ((App)Application.Current).GioRa);
                cmd.Parameters.AddWithValue("@Ngaytl", NgayTL);
                cmd.Parameters.AddWithValue("@mahd", ((App)Application.Current).MaHoaDon.ToString());
                cmd.Parameters.AddWithValue("@thanh_tien", ThanhTien);
                if (maKhachHangSdt == "0")
                    cmd.Parameters.AddWithValue("@maKhachHang", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@maKhachHang", maKhachHangSdt);
                cmd.ExecuteNonQuery();
            }
        }
        public string TimBanTrong()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TOP 1 MABAN " +
                    "FROM Ban " +
                    "WHERE TRANGTHAI = N'Trống'";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                string MB = "";
                while (reader.Read())
                {
                    MB = reader["MABAN"] as string ?? "";
                }
                return MB;
            }
        }
        public void CoKhach(string MaBan)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Ban " +
                               "SET TRANGTHAI = @TrangThai " +
                               "WHERE MABAN = @Ma";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.Add("@TrangThai", SqlDbType.NVarChar).Value = "Đang Có Khách";
                    command.Parameters.Add("@Ma",SqlDbType.VarChar).Value = MaBan;
                    command.ExecuteNonQuery();
                }
            }
        }
        public void KhachDi(string MaBan)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE Ban " +
                               "SET TRANGTHAI = @TrangThai " +
                               "WHERE MABAN = @Ma";
                using (SqlCommand command = new SqlCommand(query, conn))
                {
                    command.Parameters.Add("@TrangThai", SqlDbType.NVarChar).Value = "Trống";
                    command.Parameters.Add("@Ma", SqlDbType.VarChar).Value = MaBan;
                     command.ExecuteNonQuery();
                }
            }
        }
        public void ThemKhachHang(string MaKH)
        {
            DateTime NgayDK = DateTime.Today;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO KhachHang (MAKH, SDT, NGAYDK, SODIEMTICHLUY) " +
                               "VALUES (@Makh, @sdt, @Ngaydk, @sdtl)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Makh", MaKH);
                cmd.Parameters.AddWithValue("@sdt", MaKH);
                cmd.Parameters.AddWithValue("@Ngaydk", NgayDK);
                cmd.Parameters.AddWithValue("@sdtl", 0);
                cmd.ExecuteNonQuery();
            }
        }
        public void ThemCTHD(Food food)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO CTHD (MAHD,MAMON,SOLUONG,DONGIA,THANHTIEN) VALUES (@mahd, @mamon,@soluong,@dongia,@thanhtien)";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@mahd", ((App)Application.Current).MaHoaDon.ToString());
                cmd.Parameters.AddWithValue("@mamon", food.MAMON);
                cmd.Parameters.AddWithValue("@soluong", food.SoLuong);
                cmd.Parameters.AddWithValue("@dongia", food.GIA);
                cmd.Parameters.AddWithValue("@thanhtien", food.SoLuong*food.GIA);
                cmd.ExecuteNonQuery();
            }
        }
    }
}