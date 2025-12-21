using Microsoft.Data.SqlClient;
using System;
using System.Windows;

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
        SqlDependency.Stop(_connectionString);
        SqlDependency.Start(_connectionString);
        RegisterNotification();
    }

    public void StopListening()
    {
        SqlDependency.Stop(_connectionString);
    }

    private void RegisterNotification()
    {
        try
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                conn.Open();

                string query = "SELECT MAMON, TENMON, GIA, ANH, MOTA, CATEGORYID FROM dbo.MonAn";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Notification = null;
                    SqlDependency dependency = new SqlDependency(cmd);

                    dependency.OnChange += (sender, e) =>
                    {
                        SqlDependency dep = sender as SqlDependency;
                        if (dep != null) dep.OnChange -= (s, ev) => { };

                        if (e.Type == SqlNotificationType.Change)
                        {
                            if (Application.Current != null)
                            {
                                RegisterNotification();

                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    OnDatabaseChanged?.Invoke();
                                });
                            }
                        }
                    };


                    using (SqlDataReader reader = cmd.ExecuteReader()) { }
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("Lỗi FoodWatcher: " + ex.Message);
        }
    }
}