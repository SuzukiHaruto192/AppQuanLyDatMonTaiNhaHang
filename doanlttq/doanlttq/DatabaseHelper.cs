using Microsoft.Data.SqlClient; // ✅ Dùng Microsoft.Data.SqlClient thay vì System.Data.SqlClient
using System.Collections.Generic;
using System.Configuration;
using System.Windows;
using doanlttq.MonAn;


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
                conn.Open(); // ⚡ lỗi của bạn ở đây do connection string sai
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
                        GIA = (int)reader["GIA"],
                        ANH = duongDanAnh,
                        MOTA= (string)reader["MOTA"],
                        TRANGTHAI = (string)reader["TRANGTHAI"],
                        CATEGORYID = (string)reader["CATEGORYID"]
                    });
                }
            }

            return foods;
        }
        public List<Food> LocMonAn( string TenMon, string LoaiMon)
        {
            List<Food> foods = new List<Food>();
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                conn.Open();
                string query = "select * From MonAn ma Join Category ct on ma.CATEGORYID = ct.CATEGORYID Where ct.PARENTCATEGORYID = '"+LoaiMon+"' and TENMON like N'%"+TenMon+"%'";
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
                        GIA = (int)reader["GIA"],
                        ANH = duongDanAnh,
                        MOTA = (string)reader["MOTA"],
                        TRANGTHAI = (string)reader["TRANGTHAI"],
                        CATEGORYID = (string)reader["CATEGORYID"]
                    });
                }

            }
            return foods ;
        }
    }
}