using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace doanlttq.ChonVoucher
{
    public class Voucher 
    {
        public string MaVoucher { get; set; }
        public string TenVoucher { get; set; }
        public int SoDiem { get; set; }
        public decimal GiaTriGiam { get; set; }
        public decimal GiaTriToiThieu { get; set; }
        public Voucher() { }
        public Voucher(string mavc, string tenVoucher, int soDiem, decimal giaTriGiam, decimal giaTriToiThieu)
        {
            MaVoucher = mavc;
            TenVoucher = tenVoucher;
            SoDiem = soDiem;
            GiaTriGiam = giaTriGiam;
            GiaTriToiThieu = giaTriToiThieu;
        }
    }
}
