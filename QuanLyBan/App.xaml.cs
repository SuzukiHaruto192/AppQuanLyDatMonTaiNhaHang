using System.Configuration;
using System.Data;
using System.Windows;

namespace QuanLyBan
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string current_maban { get; set; } = string.Empty;
        public string current_mahd { get; set; } = string.Empty;
    }

}
