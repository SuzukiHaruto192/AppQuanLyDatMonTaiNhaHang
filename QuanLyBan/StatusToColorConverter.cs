using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace QuanLyBan
{
    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TableStatus status)
            {
                switch (status)
                {
                    case TableStatus.Available:
                        return new SolidColorBrush(Colors.Green);
                    case TableStatus.Occupied:
                        return new SolidColorBrush(Colors.Orange);
                    case TableStatus.Reserved:
                        return new SolidColorBrush(Colors.Blue);
                }
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
