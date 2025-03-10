using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AppQuanLyV1
{
    public class ContinueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isContinuing)
            {
                return isContinuing ? Brushes.Green : Brushes.Red;
            }
            return Brushes.Black;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
