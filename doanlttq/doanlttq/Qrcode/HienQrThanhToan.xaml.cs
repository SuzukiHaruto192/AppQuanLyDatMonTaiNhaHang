using doanlttq.Qrcode;
using PayOS;
using PayOS.Models;
using PayOS.Models.V2.PaymentRequests;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace doanlttq.Qrcode
{
    /// <summary>
    /// Interaction logic for HienQrThanhToan.xaml
    /// </summary>
    public partial class HienQrThanhToan : Window
    {
        private bool isWindowOpen = true;
        private string PAYOS_CLIENT_ID;
        private string PAYOS_API_KEY;
        private string PAYOS_CHECKSUM_KEY;
        private PayOSClient payOS;
        private long currentOrderCode;
        public HienQrThanhToan()
        {
            InitializeComponent();
            PAYOS_CLIENT_ID = "611f438a-25ab-486f-9fa0-be3fe3f44d01";
            PAYOS_API_KEY = "b30fe737-1dd3-4a70-8af7-0dc9c12c3cc6";
            PAYOS_CHECKSUM_KEY = "4eb6b1b8d2bde02558b8a9fefe4f66de0427a07f3cc981c3a81ce6a507ad2ad4";
            payOS = new PayOSClient(PAYOS_CLIENT_ID, PAYOS_API_KEY, PAYOS_CHECKSUM_KEY);
            this.Loaded += HienQrThanhToan_Loaded;
            this.Closed += (s, e) => isWindowOpen = false;
        }
        private async void HienQrThanhToan_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                currentOrderCode = long.Parse(DateTime.Now.ToString("yyMMddHHmmss"));
                int soTien = (int)((App)Application.Current).TongTien;

                var paymentRequest = new CreatePaymentLinkRequest
                {
                    OrderCode = currentOrderCode,
                    Amount = (int)soTien,
                    Description = "Don Hang So "+((App)Application.Current).MaHoaDon.ToString(),
                    CancelUrl = "http://localhost:3000/cancel",
                    ReturnUrl = "http://localhost:3000/success"
                };

                var paymentLink = await payOS.PaymentRequests.CreateAsync(paymentRequest);

                string qrString = paymentLink.QrCode;

                
                QR.Source = Qr.TaoQrTuPayOS(qrString);

                StartPollingPayment();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tạo thanh toán: " + ex.Message);
            }
        }
        private async void StartPollingPayment()
        {
            bool isPaid = false;
            while (!isPaid && isWindowOpen)
            {
                await Task.Delay(2000); 

                try
                {
                    if (currentOrderCode == 0) return;

                    var result = await payOS.PaymentRequests.GetAsync(currentOrderCode);


                    if (result != null && result.Status != null && result.Status.ToString().ToUpper() == "PAID")
                    {
                        isPaid = true;
                        Panel.SetZIndex(Grid_ThanhCong, 2);
                        XN_Button.Visibility = Visibility.Visible;
                        break;
                    }
                    else 
                        StatusText.Text = DateTime.Now.ToString();
                }
                catch (Exception ex)
                {
                    // Nếu lỗi mạng thì kệ nó, vòng lặp sau sẽ thử lại
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        private void XN_Click(object sender, RoutedEventArgs e)
        {
            login lg = new login();
            this.DialogResult = true;
            lg.Show();
            this.Close();
        }
    }
    }
