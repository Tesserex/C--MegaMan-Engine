using Avalonia;
using Avalonia.Controls.Primitives;

namespace MegaMan.Engine.Avalonia.Controls;
public class MenuHeader : TemplatedControl
{
    public static StyledProperty<string?> LabelProperty = AvaloniaProperty.Register<MenuHeader, string?>(nameof(Label));

    public string? Label
    {
        get { return GetValue(LabelProperty); }
        set { SetValue(LabelProperty, value); }
    }

    public static StyledProperty<string?> HotKeyProperty = AvaloniaProperty.Register<MenuHeader, string?>(nameof(HotKey));

    public string? HotKey
    {
        get { return GetValue(HotKeyProperty); }
        set { SetValue(HotKeyProperty, value); }
    }
}
