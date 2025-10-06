using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doanlttq.MonAn
{

        public class Food
        {
            public int MaMA { get; set; }
            public string TenMA { get; set; }
            public string Gia { get; set; }
            public string Anh { get; set; }
        public string ImagePath
        {
            get
            {
                // Dẫn tới thư mục MonAn/AnhMonAn/
                string baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
                string path = System.IO.Path.Combine(baseDir, "MonAn", "AnhMonAn", Anh ?? "");
                return path;
            }
        }
        }

        }

