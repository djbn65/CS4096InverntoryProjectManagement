using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace InventoryAndProjectManagement
{
    internal class AllocatedContentConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isAllocated)
            {
                return isAllocated ? "Allocated" : "Allocate";
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