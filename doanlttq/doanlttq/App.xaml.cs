using System.Configuration;
using System.Data;
using System.Windows;

namespace doanlttq
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public string MaHoaDon {  get; set; }
        public DateTime GioVao { get; set; }
        public DateTime GioRa { get; set; }
    }

}
