using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MegaMan.Editor
{
    public static class ThemeProperties
    {
        public static CornerRadius GetButtonRadius(DependencyObject obj)
        {
            return (CornerRadius)obj.GetValue(ButtonRadiusProperty);
        }

        public static void SetButtonRadius(DependencyObject obj, CornerRadius value)
        {
            obj.SetValue(ButtonRadiusProperty, value);
        }

        public static readonly DependencyProperty ButtonRadiusProperty =
        DependencyProperty.RegisterAttached(
            "ButtonRadius",
            typeof(CornerRadius),
            typeof(ThemeProperties),
            new FrameworkPropertyMetadata(new CornerRadius(4)));
    }
}
