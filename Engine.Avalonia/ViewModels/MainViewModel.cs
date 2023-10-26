using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using MegaMan.Engine;
using MegaMan.Engine.Avalonia;

namespace Engine.Avalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    public EngineGame? CurrentGame { get; set; }

    private int widthZoom, heightZoom, width, height;
    private bool wasMaximizedBeforeFullscreen, showDebugBar;

    private string? fpsLabel, thinkLabel, entityLabel;

    public int Width
    {
        get => width;
        set
        {
            width = value;
            OnPropertyChanged();
        }
    }

    public int Height
    {
        get => height;
        set
        {
            height = value;
            OnPropertyChanged();
        }
    }

    public bool ShowDebugBar
    {
        get => showDebugBar;
        set
        {
            showDebugBar = value;
            OnPropertyChanged();
        }
    }

    public string FpsLabel { get => fpsLabel ?? ""; set { fpsLabel = value; OnPropertyChanged(); } }
    public string ThinkLabel { get => thinkLabel ?? ""; set { thinkLabel = value; OnPropertyChanged(); } }
    public string EntityLabel { get => entityLabel ?? ""; set { SetProperty(ref entityLabel, value); } }

    public MainViewModel()
    {
        if (!Design.IsDesignMode)
        {
            CurrentGame = new EngineGame();
        }

#if DEBUG
        ShowDebugBar = true;
#endif

        widthZoom = heightZoom = 1;
        DefaultScreen();

        MegaMan.Engine.Engine.Instance.GameLogicTick += Instance_GameLogicTick;
    }

    private void DefaultScreen()
    {
        Width = Const.PixelsAcross;
        Height = Const.PixelsDown;
    }

    private void Instance_GameLogicTick(GameTickEventArgs e)
    {
        Dispatcher.UIThread.Post(() => {
            var fps = 1 / e.TimeElapsed;
            FpsLabel = $"FPS: {fps:N0} / {MegaMan.Engine.Engine.Instance.FPS}";
            ThinkLabel = "Busy: " + (MegaMan.Engine.Engine.Instance.ThinkTime * 100).ToString("N0") + "%";
            EntityLabel = "Entities: " + Game.DebugEntitiesAlive();
        });
    }
}
