using System;
using System.Globalization;
using System.Windows.Data;

namespace MegaMan.Editor.Converters
{
    public class StringComparisonConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length >= 2 && values[0] != null && values[1] != null)
            {
                return values[0].ToString() == values[1].ToString();
            }

            return false;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
