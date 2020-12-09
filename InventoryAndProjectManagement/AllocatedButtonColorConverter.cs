using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InventoryAndProjectManagement
{
    internal class AllocatedButtonColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAllocated)
            {
                return isAllocated ? Application.Current.Resources["MaterialDesignRaisedAccentButton"] : Application.Current.Resources["MaterialDesignRaisedButton"];
            }
            return "Allocate";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string isAllocatedContent)
            {
                return isAllocatedContent == "Allocated";
            }
            return false;
        }
    }
}