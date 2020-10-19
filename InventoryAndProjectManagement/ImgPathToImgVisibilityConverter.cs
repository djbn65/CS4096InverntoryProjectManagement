using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InventoryAndProjectManagement
{
    internal class ImgPathToImgVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string @string)
            {
                return (@string == "") ? Visibility.Hidden : Visibility.Visible;
            }
            return Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}