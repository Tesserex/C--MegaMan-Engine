using System;
using System.Globalization;
using System.Windows.Data;

namespace MegaMan.Editor.Controls.Converters
{
    public class PropertyToBoolConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || values[0] == null || values[1] == null)
                return false;
            else
                return values[0].Equals(values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { (value != null && value.Equals(true)) ? parameter : Binding.DoNothing };
        }
    }
}
