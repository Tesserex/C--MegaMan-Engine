using Avalonia.Controls;
using MegaMan.Engine;
using MegaMan.Engine.Avalonia;

namespace Engine.Avalonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    public EngineGame CurrentGame { get; set; }

    private int widthZoom, heightZoom, width, height;
    private bool wasMaximizedBeforeFullscreen;

    public int Width
    {
        get { return width; }
        set
        {
            width = value;
            OnPropertyChanged(nameof(Width));
        }
    }

    public int Height
    {
        get { return height; }
        set
        {
            height = value;
            OnPropertyChanged(nameof(Height));
        }
    }

    public MainViewModel()
    {
        if (!Design.IsDesignMode)
        {
            CurrentGame = new EngineGame();
        }

        widthZoom = heightZoom = 1;
        DefaultScreen();
    }

    private void DefaultScreen()
    {
        Width = Const.PixelsAcross;
        Height = Const.PixelsDown;
    }
}
