using System;
using System.Globalization;
using System.Windows.Data;

namespace InventoryAndProjectManagement
{
    internal class TitleToAbbrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string abbreviation = "";

            foreach (string word in ((string)value).Split(' '))
            {
                abbreviation += char.ToUpper(word[0]);
            }

            return abbreviation;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}