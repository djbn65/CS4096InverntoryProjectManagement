using System;
using System.Globalization;
using System.Windows.Data;

namespace InventoryAndProjectManagement
{
    internal class BooleanToOpacityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool enabled)
            {
                return enabled ? 1 : 0.5;
            }
            return 1;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double opacity)
            {
                return opacity == 1;
            }
            return true;
        }
    }
}