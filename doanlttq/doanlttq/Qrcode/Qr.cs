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
        public static BitmapImage TaoQrTuPayOS(string qrText)
        {
            // Sử dụng thư viện QRCoder để tạo ảnh từ chuỗi PayOS trả về
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrText, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            // Tạo ảnh Bitmap
            Bitmap bitmap = qrCode.GetGraphic(20);

            // Chuyển đổi Bitmap sang BitmapImage để hiển thị lên WPF
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
    }
}
