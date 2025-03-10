using System;
using System.Globalization;
using System.Windows.Data;

namespace AppQuanLyV1
{
    public class ContinueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isContinuing)
            {
                return isContinuing ? "Yes" : "No";
            }
            return "Unknown";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
