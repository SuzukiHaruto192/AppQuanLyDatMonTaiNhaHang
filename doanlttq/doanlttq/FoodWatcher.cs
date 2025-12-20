using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class FoodWatcher
{
    private string _connectionString;

    public event Action OnDatabaseChanged;

    public FoodWatcher(string connStr)
    {
        _connectionString = connStr;
    }
    public void StartListening()
    {
        SqlDependency.Start(_connectionString);
        RegisterNotification();
    }

    public void StopListening()
    {
        SqlDependency.Stop(_connectionString);
    }

    private void RegisterNotification()
    {
        using (SqlConnection conn = new SqlConnection(_connectionString))
        {
            conn.Open();
            string query = "SELECT MAMON, TENMON, GIA, ANH, MOTA, CATEGORYID FROM dbo.MonAn";

            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                SqlDependency dependency = new SqlDependency(cmd);

                dependency.OnChange += (sender, e) =>
                {
                    SqlDependency dep = sender as SqlDependency;
                    if (dep != null) dep.OnChange -= (s, ev) => { };

                    if (e.Type == SqlNotificationType.Change)
                    {
                        OnDatabaseChanged?.Invoke();

                        RegisterNotification();
                    }
                };

                cmd.ExecuteNonQuery();
            }
        }
    }
}