using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FERCO.Converters
{
    public class WidthToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width)
            {
                return new Thickness(width, 35, 0, 0);
            }
            return new Thickness(75, 35, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
