using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InventoryAndProjectManagement
{
    internal class QuantityToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int quantity)
            {
                return quantity == -1 ? Visibility.Hidden : Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}