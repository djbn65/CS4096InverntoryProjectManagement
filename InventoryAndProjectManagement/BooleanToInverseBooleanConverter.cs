using System;
using System.Globalization;
using System.Windows.Data;

namespace InventoryAndProjectManagement
{
    internal class BooleanToInverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool origVal) return !origVal;
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool origVal) return !origVal;
            return false;
        }
    }
}