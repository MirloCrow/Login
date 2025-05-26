using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FERCO.Converters
{
    public class WidthToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double width && parameter is string thresholdStr)
            {
                if (double.TryParse(thresholdStr, out double threshold))
                {
                    return width > threshold ? Visibility.Visible : Visibility.Collapsed;
                }
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}