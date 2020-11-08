using System;
using System.Globalization;
using System.Windows.Data;

namespace InventoryAndProjectManagement
{
    internal class TitleToAbbrConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string abbreviation = "";

                foreach (string word in ((string)value).Split(' '))
                {
                    if (word.Length > 0 && char.IsLetter(word[0]))
                        abbreviation += char.ToUpper(word[0]);
                }

                return abbreviation;
            }

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}