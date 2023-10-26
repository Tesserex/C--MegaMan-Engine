using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Threading;
using MegaMan.Engine;
using MegaMan.Engine.Avalonia;
using MegaMan.Engine.Input;
using Microsoft.Xna.Framework.Input;

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

    private Dictionary<Key, bool> keysPressed = new Dictionary<Key, bool>();

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

        InputElement.KeyDownEvent.AddClassHandler<TopLevel>(OnKeyDown, handledEventsToo: true);
        InputElement.KeyUpEvent.AddClassHandler<TopLevel>(OnKeyUp, handledEventsToo: true);

        GameInput.AddBinding(new KeyboardInputBinding(GameInputs.Start, Keys.Enter, k => keysPressed.GetValueOrDefault(Key.Enter)));
        GameInput.AddBinding(new KeyboardInputBinding(GameInputs.Left, Keys.Left, k => keysPressed.GetValueOrDefault(Key.Left)));
        GameInput.AddBinding(new KeyboardInputBinding(GameInputs.Up, Keys.Up, k => keysPressed.GetValueOrDefault(Key.Up)));
        GameInput.AddBinding(new KeyboardInputBinding(GameInputs.Down, Keys.Down, k => keysPressed.GetValueOrDefault(Key.Down)));
        GameInput.AddBinding(new KeyboardInputBinding(GameInputs.Right, Keys.Right, k => keysPressed.GetValueOrDefault(Key.Right)));
        GameInput.AddBinding(new KeyboardInputBinding(GameInputs.Jump, Keys.A, k => keysPressed.GetValueOrDefault(Key.A)));
        GameInput.AddBinding(new KeyboardInputBinding(GameInputs.Shoot, Keys.O, k => keysPressed.GetValueOrDefault(Key.O)));
    }

    private void OnKeyDown(object s, KeyEventArgs e)
    {
        keysPressed[e.Key] = true;
    }

    private void OnKeyUp(object s, KeyEventArgs e)
    {
        keysPressed[e.Key] = false;
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
