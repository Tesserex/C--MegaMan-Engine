using Avalonia.Controls;
using Avalonia.Interactivity;

namespace MegaMan.Engine.Avalonia.Views;
public partial class CustomNtsc : Window
{
    public CustomNtsc()
    {
        InitializeComponent();
    }

    private void CloseClicked(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}
