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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace QuanLyBan
{

    public partial class UcBan : UserControl
    {
        public TableManagementViewModel DSBan { get; set; }
        public UcBan()
        {
            InitializeComponent();
            DSBan = new TableManagementViewModel();
            this.DataContext = this;
            DSBan.RequestShowInvoice += ViewModel_RequestShowInvoice;
        }
        private void ViewModel_RequestShowInvoice(Table tableToBill)
        {
            InvoiceViewModel invoiceVM = new InvoiceViewModel(tableToBill);
            var invoiceWindow = new InvoiceWindow
            {
                DataContext = invoiceVM,
                Owner = Window.GetWindow(this)
            };

            bool? result = invoiceWindow.ShowDialog();

            if (result == true)
            {
                DatabaseHelper db = new DatabaseHelper();
                DSBan.FinalizePayment(tableToBill);

            }
        }
    }
}
