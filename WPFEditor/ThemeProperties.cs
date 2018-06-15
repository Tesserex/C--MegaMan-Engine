using System.Windows;
using System.Windows.Media;

namespace MegaMan.Editor
{
    public static class ThemeProperties
    {
        public static Brush GetFaceBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(FaceBrushProperty);
        }

        public static void SetFaceBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(FaceBrushProperty, value);
        }

        public static Brush GetShadowBrush(DependencyObject obj)
        {
            return (Brush)obj.GetValue(ShadowBrushProperty);
        }

        public static void SetShadowBrush(DependencyObject obj, Brush value)
        {
            obj.SetValue(ShadowBrushProperty, value);
        }

        public static readonly DependencyProperty FaceBrushProperty =
        DependencyProperty.RegisterAttached(
            "FaceBrush",
            typeof(Brush),
            typeof(ThemeProperties),
            new FrameworkPropertyMetadata());

        public static readonly DependencyProperty ShadowBrushProperty =
        DependencyProperty.RegisterAttached(
            "ShadowBrush",
            typeof(Brush),
            typeof(ThemeProperties),
            new FrameworkPropertyMetadata());
    }
}
