using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Checkers.Converters
{
    class BoolToBrushValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return Brushes.Brown;
            }
            return Brushes.Beige;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
