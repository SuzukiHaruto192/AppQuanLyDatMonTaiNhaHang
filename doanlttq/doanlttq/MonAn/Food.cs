using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doanlttq.MonAn
{

        public class Food
        {
            public string MAMON { get; set; }
            public string TENMON { get; set; }
            public int GIA { get; set; }
            public string ANH { get; set; }
            public string MOTA {  get; set; }
            public string TRANGTHAI {  get; set; }
            public string CATEGORYID { get; set; } 
            public int SoLuong {  get; set; }
        public string ImagePath
        {
            get
            {
                // Dẫn tới thư mục MonAn/AnhMonAn/
                string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                string path = "/MonAn/AnhMonAn" + ANH;
                return path;
            }
        }

    }

        }

