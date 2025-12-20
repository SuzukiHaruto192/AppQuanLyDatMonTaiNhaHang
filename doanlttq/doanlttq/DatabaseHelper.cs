using doanlttq.ChonVoucher;
using doanlttq.MonAn;
using Microsoft.Data.SqlClient; // ✅ Dùng Microsoft.Data.SqlClient thay vì System.Data.SqlClient
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shell;


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
                    List<string> TagsMucDich = new List<string>();
                    List<string> TagsTinhChat = new List<string>();

                    Food food = new Food();
                    food.MAMON = (string)reader["MAMON"];
                    food.TENMON = (string)reader["TENMON"];
                    food.GIA = (decimal)reader["GIA"];
                    food.ANH = duongDanAnh;
                    string[] parts = reader["TAGMUCDICH"].ToString().Split(", ");
                    foreach (string part in parts)
                        TagsMucDich.Add(part);
                    food.TagsMucDich = TagsMucDich;
                    string[] parts1 = reader["TAGTINHCHAT"].ToString().Split(", ");
                    foreach (string part in parts1)
                        TagsTinhChat.Add(part);
                    food.TagsTinhChat = TagsTinhChat;
                    food.MOTA = (string)reader["MOTA"];
                    food.CATEGORYID = (string)reader["CATEGORYID"];
                    food.SoLuong = 1;
                    foods.Add(food);
                }
            }

            return foods;
        }

        public List<Food> TimMon(string TenMon, string LoaiMon)
        {
            List<Food> foods = new List<Food>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "select ma.MAMON , ma.TENMON , ma.GIA , ma.ANH , ma.MOTA , ma.CATEGORYID " +
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
                string query = "select TOP 1 MAHD FROM HoaDon ORDER BY NGAYTL DESC , GIOVAO DESC";
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
        public void ThemHoaDon(decimal ThanhTien) // VTieu CÓ CHỈNH SỬA
        {
            DateTime NgayTL = DateTime.Today;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO HoaDon (GIOVAO, GIORA, NGAYTL, MAHD, THANHTIEN, MAKH , TRANGTHAI , MABAN , TAMTINH) " +
                               "VALUES (@gio_vao, @gio_ra, @Ngaytl, @mahd, @thanh_tien, @maKhachHang , @tt , @mb , @thanh_tien)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@gio_vao", ((App)Application.Current).GioVao);
                cmd.Parameters.AddWithValue("@gio_ra", DBNull.Value);
                cmd.Parameters.AddWithValue("@Ngaytl", NgayTL);
                cmd.Parameters.AddWithValue("@mahd", ((App)Application.Current).MaHoaDon.ToString());
                cmd.Parameters.AddWithValue("@thanh_tien", ThanhTien);
                cmd.Parameters.AddWithValue("@tt", "Chưa thanh toán");
                cmd.Parameters.AddWithValue("@mb", ((App)Application.Current).MABAN);
                if (((App)Application.Current).MaKH == "0")
                    cmd.Parameters.AddWithValue("@maKhachHang", DBNull.Value);
                else
                    cmd.Parameters.AddWithValue("@maKhachHang", ((App)Application.Current).MaKH);
                cmd.ExecuteNonQuery();
            }
        }
        public void UpdateHoaDon(decimal TamTinh, DateTime? DT, string TrangThai, decimal GiamGia = 0) // VT CÓ CHỈNH SỬA
        {
            DateTime NgayTL = DateTime.Today;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE HoaDon " +
                               "SET TAMTINH = @tamtinh , GIAMGIA = @giamgia , GIORA = @gio_ra , TRANGTHAI = @tt , THANHTIEN = @tamtinh - @giamgia " +
                               "WHERE MAHD = @mahd";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@gio_ra", DT ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@mahd", ((App)Application.Current).MaHoaDon.ToString());
                    cmd.Parameters.AddWithValue("@tamtinh", TamTinh);
                    cmd.Parameters.AddWithValue("@giamgia", GiamGia);
                    cmd.Parameters.AddWithValue("@tt", TrangThai);
                    //if (maKhachHangSdt == "0")
                    //    cmd.Parameters.AddWithValue("@makh", DBNull.Value);
                    //else
                    //    cmd.Parameters.AddWithValue("@makh", maKhachHangSdt);
                    cmd.ExecuteNonQuery();
                }
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
                    command.Parameters.Add("@TrangThai", SqlDbType.NVarChar).Value = "Đang phục vụ";
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

        // TÍCH ĐIỂM
        public void ThemKhachHang(string MaKH)
        {
            DateTime NgayDK = DateTime.Today;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "INSERT INTO KhachHang (MAKH, NGAYDK, SODIEMTICHLUY) " +
                               "VALUES (@Makh, @Ngaydk, @sdtl)";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Makh", MaKH);
                cmd.Parameters.AddWithValue("@Ngaydk", NgayDK);
                cmd.Parameters.AddWithValue("@sdtl", 0);
                cmd.ExecuteNonQuery();
            }
        }
        public void TichDiem(string MaKH, int Diem)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "UPDATE KhachHang SET SODIEMTICHLUY = SODIEMTICHLUY + @Diem WHERE MAKH = @Makh";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Makh", MaKH);
                cmd.Parameters.AddWithValue("@Diem", Diem);
                cmd.ExecuteNonQuery();
            }
        }
        public int LayDiemTichLuy(string MaKH)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT SODIEMTICHLUY FROM KhachHang WHERE MAKH = @Makh";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Makh", MaKH);
                SqlDataReader reader = cmd.ExecuteReader();
                int diemtl = 0;
                while (reader.Read())
                {
                    diemtl = Convert.ToInt32(reader["SODIEMTICHLUY"]);
                }
                return diemtl;
            }
        }
        public List<Voucher> LayVoucher(decimal tongtien, int diem)
        {
            List<Voucher> vouchers = new List<Voucher>();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT MAVC, TENVC, SODIEMCANDOI, SOTIENDUOCGIAM, GIATRIHOADONTOITHIEU FROM Voucher " +
                               "WHERE SODIEMCANDOI <= @Diem and GIATRIHOADONTOITHIEU <= @Tien " +
                               "ORDER BY SOTIENDUOCGIAM DESC";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@Diem", diem);
                cmd.Parameters.AddWithValue("@Tien", tongtien);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Console.WriteLine(reader["TENVC"]);
                    vouchers.Add(new Voucher
                    {
                        MaVoucher = (string)reader["MAVC"],
                        TenVoucher = (string)reader["TENVC"],
                        SoDiem = Convert.ToInt32(reader["SODIEMCANDOI"]),
                        GiaTriGiam = Convert.ToDecimal(reader["SOTIENDUOCGIAM"]),
                        GiaTriToiThieu = Convert.ToDecimal(reader["GIATRIHOADONTOITHIEU"])
                    });
                }

            }
            return vouchers;
        }
        // END TICH ĐIỂM
        public void ThemCTHD(Food food)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();

                // Câu lệnh SQL: Nếu tồn tại thì Update cộng dồn, nếu chưa thì Insert
                string query = @"
            IF EXISTS (SELECT 1 FROM CTHD WHERE MAHD = @mahd AND MAMON = @mamon)
            BEGIN
                UPDATE CTHD 
                SET SOLUONG = SOLUONG + @soluong,
                    THANHTIEN = (SOLUONG + @soluong) * @dongia -- Tính lại thành tiền dựa trên số lượng mới
                WHERE MAHD = @mahd AND MAMON = @mamon
            END
            ELSE
            BEGIN
                INSERT INTO CTHD (MAHD, MAMON, SOLUONG, DONGIA, THANHTIEN) 
                VALUES (@mahd, @mamon, @soluong, @dongia, @thanhtien)
            END";

                SqlCommand cmd = new SqlCommand(query, conn);

                // Khai báo các tham số
                cmd.Parameters.AddWithValue("@mahd", ((App)Application.Current).MaHoaDon.ToString());
                cmd.Parameters.AddWithValue("@mamon", food.MAMON);
                cmd.Parameters.AddWithValue("@soluong", food.SoLuong); // Số lượng khách vừa chọn thêm
                cmd.Parameters.AddWithValue("@dongia", food.GIA);
                cmd.Parameters.AddWithValue("@thanhtien", food.SoLuong * food.GIA); // Thành tiền của lần thêm này

                cmd.ExecuteNonQuery();
            }
        }
        public ObservableCollection<Food> LayCTHD()
        {
            ObservableCollection<Food> Foods = new ObservableCollection<Food>();

            void LoadDataRealTime()
            {
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        conn.Open();
                        string query = @"
                    SELECT ct.SOLUONG, ma.MAMON, ma.TENMON, ma.GIA, ma.ANH, 
                           ma.MOTA, ma.CATEGORYID
                    FROM dbo.MONAN ma 
                    JOIN dbo.CTHD ct ON ma.MAMON = ct.MAMON
                    WHERE ct.MAHD = @mahd";
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@mahd", ((App)Application.Current).MaHoaDon.ToString());

                            SqlDependency dependency = new SqlDependency(cmd);

                            // Sự kiện: Khi DB thay đổi -> Gọi lại hàm LoadDataRealTime
                            dependency.OnChange += (sender, e) =>
                            {
                                cmd.Notification = null;
                                SqlDependency dep = sender as SqlDependency;
                                dep.OnChange -= (s, ev) => { };

                                // --- LOGIC GỐC CỦA BẠN ---
                                if (e.Type == SqlNotificationType.Change)
                                {
                                    Application.Current.Dispatcher.Invoke(LoadDataRealTime);
                                }
                                // --- THÊM ĐOẠN NÀY ĐỂ BẮT LỖI ---
                                else
                                {
                                    // Nếu chạy vào đây tức là SQL TỪ CHỐI theo dõi
                                    Application.Current.Dispatcher.Invoke(() => {
                                        MessageBox.Show($"SQL Từ Chối Theo Dõi!\nLý do: {e.Info}\nLoại: {e.Type}");
                                    });
                                }
                            };

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                // Tạo list tạm để hứng dữ liệu sạch
                                var tempList = new List<Food>();

                                while (reader.Read())
                                {
                                    string tenFileAnh = reader["ANH"] as string ?? "";
                                    string duongDanAnh = "MonAn/AnhMonAn/" + tenFileAnh;

                                    tempList.Add(new Food
                                    {
                                        MAMON = reader["MAMON"].ToString(), 
                                        TENMON = reader["TENMON"].ToString(),
                                        GIA = Convert.ToDecimal(reader["GIA"]),
                                        ANH = duongDanAnh,
                                        MOTA = reader["MOTA"].ToString(),
                                        CATEGORYID = reader["CATEGORYID"].ToString(),
                                        SoLuong = Convert.ToInt32(reader["SOLUONG"])
                                    });
                                }

                                // --- C. CẬP NHẬT UI ---
                                // Update vào biến 'Foods' đã khai báo ở đầu hàm
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    Foods.Clear(); // Xóa dữ liệu cũ
                                    foreach (var item in tempList)
                                    {
                                        Foods.Add(item); // Thêm dữ liệu mới
                                    }
                                });
                            }
                        }
                    }
            }

            // 3. Kích hoạt hàm load lần đầu tiên
            LoadDataRealTime();
            return Foods;
        }

        public List<string> GetTagTinhChat(string MaMon)
        {
            List<string> TagsTinhChat = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TAGTINHCHAT FROM MONAN WHERE MAMON = @MaMon";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaMon", MaMon);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] parts = reader["TAGTINHCHAT"].ToString().Split(", ");
                            foreach (string part in parts)
                            {
                                TagsTinhChat.Add(part);
                            }
                        }

                    }
                }
            }
            return TagsTinhChat;
        }

        public List<string> GetTagMucDich(string MaMon)
        {
            List<string> TagsMucDich = new List<string>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT TAGMUCDICH FROM MONAN WHERE MAMON = @MaMon";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@MaMon", MaMon);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string[] parts = reader["TAGMUCDICH"].ToString().Split(", ");
                            foreach (string part in parts)
                            {
                                TagsMucDich.Add(part);
                            }
                        }

                    }
                }
            }
            return TagsMucDich;
        }
    }
}
