

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace MegaMan.Engine.Avalonia;

public class ThemeProperties : AvaloniaObject
{
    public static Brush GetFaceBrush(AvaloniaObject obj)
    {
        return obj.GetValue(FaceBrushProperty);
    }

    public static void SetFaceBrush(AvaloniaObject obj, Brush value)
    {
        obj.SetValue(FaceBrushProperty, value);
    }

    public static Brush GetShadowBrush(AvaloniaObject obj)
    {
        return obj.GetValue(ShadowBrushProperty);
    }

    public static void SetShadowBrush(AvaloniaObject obj, Brush value)
    {
        obj.SetValue(ShadowBrushProperty, value);
    }

    public static readonly StyledProperty<Brush> FaceBrushProperty =
    AvaloniaProperty.RegisterAttached<ThemeProperties, Control, Brush>("FaceBrush");

    public static readonly StyledProperty<Brush> ShadowBrushProperty =
    AvaloniaProperty.RegisterAttached<ThemeProperties, Control, Brush>("ShadowBrush");
}
