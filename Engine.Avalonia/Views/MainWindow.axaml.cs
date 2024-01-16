using System;
using Avalonia;
using Avalonia.Controls;
using MegaMan.Engine.Avalonia.ViewModels;

namespace MegaMan.Engine.Avalonia.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

#if DEBUG
        this.AttachDevTools();
#endif
    }

    protected override void OnClosing(WindowClosingEventArgs e)
    {
        base.OnClosing(e);
        (this.DataContext as MainViewModel)?.Quit();
    }

    

    protected override void OnResized(WindowResizedEventArgs e)
    {
        base.OnResized(e);
        if (DataContext is MainViewModel viewModel && e.Reason == WindowResizeReason.User)
        {
            viewModel.ManualSize();
        }
    }
}
