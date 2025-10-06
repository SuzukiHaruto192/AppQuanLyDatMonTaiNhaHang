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
                string query = "SELECT MaMA, TenMA, Gia, Anh FROM Foods";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string tenFileAnh = reader["Anh"].ToString();
                    string duongDanAnh = "MonAn/AnhMonAn/" + tenFileAnh;
                    foods.Add(new Food
                    {
                        MaMA = (int)reader["MaMA"],
                        TenMA = reader["TenMA"].ToString(),
                        Gia = reader["Gia"].ToString(),
                        Anh = duongDanAnh
                    });
                }
            }

            return foods;
        }
        public List<Food> LocMonAn( string TenMon)
        {
            List<Food> foods = new List<Food>();
            using (SqlConnection conn = new SqlConnection(connectionString)) {
                conn.Open();
                string query = "SELECT * FROM FOODS WHERE TenMA like N'%" + TenMon + "%'";
                SqlCommand cmd = new SqlCommand(query, conn);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read()) {
                    string TenFileAnh = reader["Anh"].ToString();
                    String duongdan = "MonAn/AnhMonAn/" + TenFileAnh;
                    foods.Add(new Food {
                        MaMA = (int)reader["MaMA"],
                        TenMA = reader["TenMA"].ToString(),
                        Gia = reader["Gia"].ToString(),
                        Anh = duongdan
                    });
                }

            }
            return foods ;
        }
    }
}