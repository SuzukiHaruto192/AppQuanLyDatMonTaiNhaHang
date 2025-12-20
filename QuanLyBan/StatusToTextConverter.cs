using QuanLyBan.Ban;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace QuanLyBan
{
    public class StatusToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TableStatus status)
            {
                switch (status)
                {
                    case TableStatus.Available:
                        return "Trống";
                    case TableStatus.Occupied:
                        return "Đang phục vụ";
                    case TableStatus.Reserved:
                        return "Đã đặt trước";
                }
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
