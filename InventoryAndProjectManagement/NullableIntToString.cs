using System;
using System.Globalization;
using System.Windows.Data;

namespace InventoryAndProjectManagement
{
    internal class NullableIntToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string @string)
            {
                return (@string == "") ? (int?)null : int.Parse(@string);
            }
            return null;
        }
    }
}