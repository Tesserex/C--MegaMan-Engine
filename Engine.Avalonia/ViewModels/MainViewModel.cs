using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.Metadata;
using System.Text;
using System.Windows.Input;
using System.Xml;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using MegaMan.Engine;
using MegaMan.Engine.Avalonia;
using MegaMan.Engine.Input;
using MegaMan.IO.Xml;
using Microsoft.Xna.Framework.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using static System.Net.Mime.MediaTypeNames;

namespace Engine.Avalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    public EngineGame? CurrentGame { get; set; }

    private int widthZoom, heightZoom, width, height;
    private bool wasMaximizedBeforeFullscreen, showDebugBar;

    private string? fpsLabel, thinkLabel, entityLabel;

    private string windowTitle;

    public string WindowTitle { get { return windowTitle; } set { SetProperty(ref windowTitle, value); } }

    private bool pausedFromMenu;
    public bool PausedFromMenu { get { return pausedFromMenu; } set { SetProperty(ref pausedFromMenu, value); } }

    private string? lastGameWithPath;

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

    public ICommand ResetGameCommand { get; }
    public ICommand CloseGameCommand { get; }
    public ICommand QuitCommand { get; }
    public ICommand PauseCommand { get; }

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

        ResetGameCommand = new RelayCommand(ResetGame);
        CloseGameCommand = new RelayCommand(CloseGame);
        QuitCommand = new RelayCommand(Quit);
        PauseCommand = new RelayCommand(Pause);
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

    private void ResetGame()
    {
        if (Game.CurrentGame != null)
        {
            Game.CurrentGame.Reset();
            OnGameLoaded();
        }
    }

    private void CloseGame()
    {
        if (Game.CurrentGame != null)
        {
            AutosaveConfig();
            lastGameWithPath = null;
            //LoadCurrentConfig();

            Game.CurrentGame.Unload();
            WindowTitle = "Mega Man";

            OnGameLoadedChanged();
        }
    }

    private void Quit()
    {
        CloseApp();
        if (global::Avalonia.Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.TryShutdown();
        }
    }

    /// <summary>
    /// Things to do when closing engine, no matter the way
    /// </summary>
    public void CloseApp()
    {
        AutosaveConfig();
        if (Game.CurrentGame != null) Game.CurrentGame.Unload();
    }

    private void Pause()
    {
        PausedFromMenu = !PausedFromMenu;

        if (PausedFromMenu)
            MegaMan.Engine.Engine.Instance.Pause();
        else
            MegaMan.Engine.Engine.Instance.Unpause();
    }

    private void AutosaveConfig(string? fileName = null)
    {
        // if (autosaveToolStripMenuItem.Checked) SaveConfig();
    }

    public void LoadFromOpenDialog(string filename)
    {
        AutosaveConfig();
        //SaveGlobalConfigValues();

        LoadGame(filename);
    }

    private bool LoadGame(string path, List<string> pathArgs = null, bool silenceErrorMessages = false)
    {
        try
        {
            Game.Load(path, pathArgs);
            WindowTitle = Game.CurrentGame.Name;

            lastGameWithPath = path;
            //LoadCurrentConfig();

            OnGameLoadedChanged();

            // var userSettings = settingsService.GetSettings();
            // userSettings.AddRecentGame(Game.CurrentGame.Name, path);
            // XML.SaveToConfigXML(userSettings, settingsService.SettingsFilePath);

            return true;
        }
        catch (GameXmlException ex)
        {
            if (silenceErrorMessages == false)
            {
                // this builds a dialog message to tell the user where the error is in the XML file

                var message = new StringBuilder("There is an error in one of your game files.\n\n");
                if (ex.File != null) message.Append("File: ").Append(ex.File).Append('\n');
                if (ex.Line != 0) message.Append("Line: ").Append(ex.Line.ToString()).Append('\n');
                if (ex.Entity != null) message.Append("Entity: ").Append(ex.Entity).Append('\n');
                if (ex.Tag != null) message.Append("Tag: ").Append(ex.Tag).Append('\n');
                if (ex.Attribute != null) message.Append("Attribute: ").Append(ex.Attribute).Append('\n');

                message.Append("\n").Append(ex.Message);

                MessageBoxManager.GetMessageBoxStandard("Game Load Error", message.ToString(), ButtonEnum.Ok, Icon.Error);
            }
            Game.CurrentGame.Unload();
        }
        catch (FileNotFoundException ex)
        {
            if (silenceErrorMessages == false)
            {
                MessageBoxManager.GetMessageBoxStandard("C# MegaMan Engine", "I'm sorry, I couldn't the following file. Perhaps the file path is incorrect?\n\n" + ex.Message, ButtonEnum.Ok, Icon.Error);
            }
            Game.CurrentGame.Unload();
        }
        catch (XmlException ex)
        {
            if (silenceErrorMessages == false)
            {
                MessageBoxManager.GetMessageBoxStandard("C# MegaMan Engine", "Your XML is badly formatted.\n\nFile: " + ex.SourceUri + "\n\nError: " + ex.Message, ButtonEnum.Ok, Icon.Error);
            }
            Game.CurrentGame.Unload();
        }
#if !DEBUG
            catch (Exception ex)
            {
                if (silenceErrorMessages == false)
                {
                    MessageBoxManager.GetMessageBoxStandard("C# MegaMan Engine", "There was an error loading the game.\n\n" + ex.Message, ButtonEnum.Ok, Icon.Error);

                    // Get stack trace for the exception with source file information
                    var st = new StackTrace(ex, true);
                    // Get the top stack frame
                    var frame = st.GetFrame(0);
                    // Get the line number from the stack frame
                    var line = frame?.GetFileLineNumber();

                    MessageBoxManager.GetMessageBoxStandard("C# MegaMan Engine", "StackTrace: " + st + " Frame: " + frame + " Line: " + line, ButtonEnum.Ok, Icon.Error);

                }
                Game.CurrentGame.Unload();
            }
#endif
        // Only call if if current form is the active one
        //if (GetForegroundWindow() == Handle)
        //{
        //    OnActivated(new EventArgs());
        //}

        return false;
    }

    private void OnGameLoadedChanged()
    {
        //LoadGlobalConfigValues();
        OnGameLoaded();
    }

    private void OnGameLoaded()
    {
        //SetLayersVisibilityFromSettings();
    }
}
