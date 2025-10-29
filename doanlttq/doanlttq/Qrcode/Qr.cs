using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace doanlttq.Qrcode
{
    public class Qr
    {
        public static BitmapImage TaoQr (string NganHang, string STK, string ChuTaiKhoan , decimal SoTien , string GhiChu)
        {
            string url = $"https://img.vietqr.io/image/{NganHang}-{STK}-compact.png?amount={SoTien}&addInfo={Uri.EscapeDataString(GhiChu)}&accountName={Uri.EscapeDataString(ChuTaiKhoan)}";

            BitmapImage qrImage = new BitmapImage();
            qrImage.BeginInit();
            qrImage.UriSource = new Uri(url, UriKind.Absolute);
            qrImage.EndInit();
            return qrImage;
        }
    }
}
