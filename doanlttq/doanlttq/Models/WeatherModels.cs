using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doanlttq.Models
{
    public class Weather
    {
        public string main { get; set; }
    }

    public class MainData
    {
        public float temp { get; set; }
    }

    public class WeatherApiResponse
    {
        public List<Weather> weather { get; set; } = new List<Weather>();
        public MainData main { get; set; } = new MainData();
    }
}
