﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using MegaMan.Engine.Avalonia.Settings;
using MegaMan.Engine.Input;
using MegaMan.IO.Xml;
using Microsoft.Xna.Framework.Input;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using KeyboardInputBinding = MegaMan.Engine.Avalonia.Settings.KeyboardInputBinding;

namespace MegaMan.Engine.Avalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    public EngineGame? CurrentGame { get; set; }

    private int widthZoom, heightZoom, width, height;
    private bool wasMaximizedBeforeFullscreen, showDebugBar;

    private string? fpsLabel, thinkLabel, entityLabel;

    private string windowTitle = "Mega Man Engine";
    public string WindowTitle { get => windowTitle; set { SetProperty(ref windowTitle, value); } }

    private WindowState windowState;
    public WindowState WindowState { get => windowState; set { SetProperty(ref windowState, value); } }

    private bool pausedFromMenu;
    public bool PausedFromMenu { get { return pausedFromMenu; } set { SetProperty(ref pausedFromMenu, value); } }

    private string? lastGameWithPath;

    public string? InitialFolder { get; private set; }

    public IEnumerable<RecentGame> RecentGames
    {
        get
        {
            var userSettings = settingsService.GetSettings();
            if (userSettings.RecentGames != null)
            {
                return userSettings.RecentGames;
            }
            return Enumerable.Empty<RecentGame>();
        }
    }

    private PixelPoint position;
    public PixelPoint Position { get => position; set { SetProperty(ref position, value); } }

    public int Width { get => width; set { SetProperty(ref width, value); } }

    public int Height { get => height; set { SetProperty(ref height, value); } }

    private readonly SettingsService settingsService;

    public bool ShowDebugBar
    {
        get => showDebugBar;
        set { showDebugBar = value; OnPropertyChanged(); }
    }

    public bool MusicEnabled
    {
        get => Engine.Instance.SoundSystem.MusicEnabled;
        set { Engine.Instance.SoundSystem.MusicEnabled = value; OnPropertyChanged(); }
    }

    public bool SfxEnabled
    {
        get => Engine.Instance.SoundSystem.SfxEnabled;
        set { Engine.Instance.SoundSystem.SfxEnabled = value; OnPropertyChanged(); }
    }

    public bool SquareOneEnabled
    {
        get => Engine.Instance.SoundSystem.SquareOne;
        set { Engine.Instance.SoundSystem.SquareOne = value; OnPropertyChanged(); }
    }

    public bool SquareTwoEnabled
    {
        get => Engine.Instance.SoundSystem.SquareTwo;
        set { Engine.Instance.SoundSystem.SquareTwo = value; OnPropertyChanged(); }
    }

    public bool TriangleEnabled
    {
        get => Engine.Instance.SoundSystem.Triangle;
        set { Engine.Instance.SoundSystem.Triangle = value; OnPropertyChanged(); }
    }

    public bool NoiseEnabled
    {
        get => Engine.Instance.SoundSystem.Noise;
        set { Engine.Instance.SoundSystem.Noise = value; OnPropertyChanged(); }
    }

    public bool ShowHitboxes
    {
        get => Engine.Instance.DrawHitboxes;
        set { Engine.Instance.DrawHitboxes = value; OnPropertyChanged(); }
    }

    public bool Invincibility
    {
        get => Engine.Instance.Invincible;
        set { Engine.Instance.Invincible = value; OnPropertyChanged(); }
    }

    public bool NoDamage
    {
        get => Engine.Instance.NoDamage;
        set { Engine.Instance.NoDamage = value; OnPropertyChanged(); }
    }

    public string FpsLabel { get => fpsLabel ?? ""; set { SetProperty(ref fpsLabel, value); } }
    public string ThinkLabel { get => thinkLabel ?? ""; set { SetProperty(ref thinkLabel, value); } }
    public string EntityLabel { get => entityLabel ?? ""; set { SetProperty(ref entityLabel, value); } }

    private bool useDefaultConfig;
    public bool UseDefaultConfig { get => useDefaultConfig; set { SetProperty(ref useDefaultConfig, value); } }

    // whether configurations should autosave
    private bool autosave;
    public bool Autosave { get => autosave; set { SetProperty(ref autosave, value); } }

    // whether a game should autoload on app load
    private bool autoload;
    public bool Autoload { get => autoload; set { SetProperty(ref autoload, value); } }

    public ICommand ResetGameCommand { get; }
    public ICommand CloseGameCommand { get; }
    public ICommand QuitCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand OpenRecentCommand { get; }
    public ICommand AutosaveCommand { get; }
    public ICommand AutoloadCommand { get; }
    public ICommand OpenBindingsCommand { get; }
    public ICommand ToggleMusicCommand { get; }
    public ICommand ToggleSfxCommand { get; }
    public ICommand ToggleSquareOneCommand { get; }
    public ICommand ToggleSquareTwoCommand { get; }
    public ICommand ToggleTriangleCommand { get; }
    public ICommand ToggleNoiseCommand { get; }
    public ICommand IncreaseVolumeCommand { get; }
    public ICommand DecreaseVolumeCommand { get; }

    private string CurrentGamePath
    {
        get { return Game.CurrentGame != null ? Game.CurrentGame.BasePath : string.Empty; }
    }

    private string CurrentGameTitle
    {
        get { return Game.CurrentGame != null ? Game.CurrentGame.Name : string.Empty; }
    }

    public MainViewModel()
    {
        if (!Design.IsDesignMode)
        {
            CurrentGame = new EngineGame();
        }

        settingsService = new SettingsService();

#if DEBUG
        ShowDebugBar = true;
#endif

        widthZoom = heightZoom = 1;
        DefaultScreen();

        Engine.Instance.GameLogicTick += Instance_GameLogicTick;

        ResetGameCommand = new RelayCommand(ResetGame);
        CloseGameCommand = new RelayCommand(CloseGame);
        QuitCommand = new RelayCommand(Quit);
        PauseCommand = new RelayCommand(Pause);
        OpenRecentCommand = new RelayCommand<string?>(path => {
            if (path is not null) LoadGame(path);
        }, path => path is not null);
        AutosaveCommand = new RelayCommand(AutosaveChanged);
        AutoloadCommand = new RelayCommand(AutoloadChanged);

        OpenBindingsCommand = new RelayCommand(() => { });

        ToggleMusicCommand = new RelayCommand(() => MusicEnabled = !MusicEnabled);
        ToggleSfxCommand = new RelayCommand(() => SfxEnabled = !SfxEnabled);
        ToggleSquareOneCommand = new RelayCommand(() => SquareOneEnabled = !SquareOneEnabled);
        ToggleSquareTwoCommand = new RelayCommand(() => SquareTwoEnabled = !SquareTwoEnabled);
        ToggleTriangleCommand = new RelayCommand(() => TriangleEnabled = !TriangleEnabled);
        ToggleNoiseCommand = new RelayCommand(() => NoiseEnabled = !NoiseEnabled);
        IncreaseVolumeCommand = new RelayCommand(() => Engine.Instance.SoundSystem.Volume++);
        DecreaseVolumeCommand = new RelayCommand(() => Engine.Instance.SoundSystem.Volume--);
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
            FpsLabel = $"FPS: {fps:N0} / {Engine.Instance.FPS}";
            ThinkLabel = "Busy: " + (Engine.Instance.ThinkTime * 100).ToString("N0") + "%";
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
            LoadCurrentConfig();

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
            Engine.Instance.Pause();
        else
            Engine.Instance.Unpause();
    }

    private void AutosaveChanged()
    {
        Autosave = !Autosave;
        //SaveGlobalConfigValues();
    }

    private void AutoloadChanged()
    {
        if (string.IsNullOrEmpty(CurrentGamePath)) Autoload = true;
        else
        {
            Autoload = !Autoload;
        }

        SaveGlobalConfigValues();
    }

    private void SaveGlobalConfigValues(string? fileName = null)
    {
        var userSettings = settingsService.GetSettings();

        userSettings.AutosaveSettings = Autosave;
        userSettings.UseDefaultSettings = UseDefaultConfig;
        userSettings.Autoload = Autoload ? lastGameWithPath : null;
        userSettings.InitialFolder = InitialFolder;

        XML.SaveToConfigXML(userSettings, settingsService.SettingsFilePath, fileName);
    }

    private void AutosaveConfig(string? fileName = null)
    {
        if (Autosave) SaveConfig();
    }

    /// <summary>
    /// When engine is opening, every cases of bad files are handled, then file is locked.
    /// It means that calling this function after this call, it will always be valid since only program can modify it.
    /// </summary>
    /// <param name="fileName"></param>
    /// <param name="settings"></param>
    private void SaveConfig(string? fileName = null, Setting? settings = null)
    {
        if (settings == null)
        {
            settings = new Setting {
                GameFileName = UseDefaultConfig ? "" : CurrentGamePath,
                GameTitle = UseDefaultConfig ? "" : CurrentGameTitle,
                KeyBindings = GetKeyBindingSettings(),
                JoystickBindings = GetJoystickBindingSettings(),
                GamepadBindings = GetGamepadBindingSettings(),
                ActiveInput = GameInput.ActiveType,
                Screens = new LastScreen {
                    Maximized = WindowState == WindowState.Maximized,
                    //NTSC_Custom = customNtscForm.GetOptions(),
                    //HideMenu = hideMenuItem.Checked
                },
                Audio = new LastAudio {
                    Volume = Engine.Instance.SoundSystem.Volume,
                    Musics = MusicEnabled,
                    Sound = SfxEnabled
                },
                Debug = new LastDebug {
                    ShowMenu = ShowDebugBar,
                    ShowHitboxes = ShowHitboxes,
                    Framerate = Engine.Instance.FPS,
                    Cheat = new LastCheat {
                        Invincibility = Invincibility,
                        NoDamage = NoDamage
                    }
                },
                Miscellaneous = new LastMiscellaneous {
                    ScreenX_Coordinate = Position.X,
                    ScreenY_Coordinate = Position.Y
                }
            };

            //foreach (var c in controllers)
            //    c.SaveSettings(settings);
        }

        var userSettings = settingsService.GetSettings();
        userSettings.AddOrSetExistingSettingsForGame(settings);

        XML.SaveToConfigXML(userSettings, settingsService.SettingsFilePath, fileName);
    }

    private List<UserKeyBindingSetting> GetKeyBindingSettings()
    {
        return GameInput.GetBindingsOf<KeyboardInputBinding>().Select(x => new UserKeyBindingSetting { Input = x.Input, Key = x.Key }).ToList();
    }

    private List<UserJoystickBindingSetting> GetJoystickBindingSettings()
    {
        return GameInput.GetJoystickBindings().Select(x => new UserJoystickBindingSetting { Input = x.Input, DeviceGuid = x.DeviceGuid, Button = x.Button, Value = x.Value }).ToList();
    }

    private List<UserGamepadBindingSetting> GetGamepadBindingSettings()
    {
        return GameInput.GetGamepadBindings().Select(x => new UserGamepadBindingSetting { Input = x.Input, Button = x.Button }).ToList();
    }

    public void LoadFromOpenDialog(string filename)
    {
        AutosaveConfig();
        //SaveGlobalConfigValues();

        LoadGame(filename);
    }

    private bool LoadGame(string path, List<string>? pathArgs = null, bool silenceErrorMessages = false)
    {
        try
        {
            Game.Load(path, pathArgs);
            WindowTitle = Game.CurrentGame.Name;

            lastGameWithPath = path;
            LoadCurrentConfig();

            OnGameLoadedChanged();

            var userSettings = settingsService.GetSettings();
            userSettings.AddRecentGame(Game.CurrentGame.Name, path);
            XML.SaveToConfigXML(userSettings, settingsService.SettingsFilePath);

            return true;
        }
        catch (GameXmlException ex)
        {
            if (silenceErrorMessages == false)
            {
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

    private void LoadCurrentConfig()
    {
        if (UseDefaultConfig)
            LoadConfigFromSetting(settingsService.GetConfigForGame(""));
        else
            LoadConfigFromSetting(settingsService.GetConfigForGame(CurrentGamePath));
    }

    private void LoadConfigFromSetting(Setting settings)
    {
        #region Input Menu: Keys
        foreach (var binding in settings.KeyBindings)
        {
            GameInput.AddBinding(binding.GetGameInputBinding());
        }
        foreach (var binding in settings.JoystickBindings)
        {
            GameInput.AddBinding(binding.GetGameInputBinding());
        }
        foreach (var binding in settings.GamepadBindings)
        {
            GameInput.AddBinding(binding.GetGameInputBinding());
        }
        GameInput.ActiveType = settings.ActiveInput;
        #endregion

        #region Screen Menu
        // NTSC option is set before. So if menu selected is NTSC, options are set.
        if (!Enum.IsDefined(typeof(NTSC_Options), settings.Screens.NTSC_Options))
        {
            WrongConfigAlert(ConfigFileInvalidValuesMessages.NTSC_Option);
            settings.Screens.NTSC_Options = UserSettings.Default.Screens.NTSC_Options;
        }

        //xnaImage.ntscInit(new snes_ntsc_setup_t(settings.Screens.NTSC_Custom));
        //customNtscForm.SetOptions(settings.Screens.NTSC_Custom);

        if (!Enum.IsDefined(typeof(ScreenScale), settings.Screens.Size))
        {
            WrongConfigAlert(ConfigFileInvalidValuesMessages.Size);
            settings.Screens.Size = UserSettings.Default.Screens.Size;
        }

        if (!Enum.IsDefined(typeof(PixellatedOrSmoothed), settings.Screens.Pixellated))
        {
            WrongConfigAlert(ConfigFileInvalidValuesMessages.PixellatedOrSmoothed);
            settings.Screens.Pixellated = UserSettings.Default.Screens.Pixellated;
        }

        //hideMenu(settings.Screens.HideMenu);

        if (settings.Screens.Maximized) WindowState = WindowState.Maximized;
        #endregion

        #region Audio Menu
        //SetVolume(settings.Audio.Volume);
        //setMusic(settings.Audio.Musics);
        //setSFX(settings.Audio.Sound);
        #endregion

        #region Debug Menu
#if DEBUG
        ShowDebugBar = settings.Debug.ShowMenu;
        ShowHitboxes = settings.Debug.ShowHitboxes;
        Engine.Instance.FPS = settings.Debug.Framerate;

        #region Cheats
        Invincibility = settings.Debug.Cheat.Invincibility;
        NoDamage = settings.Debug.Cheat.NoDamage;
        #endregion
#else
        ShowDebugBar = UserSettings.Default.Debug.ShowMenu;
        ShowHitboxes = UserSettings.Default.Debug.ShowHitboxes;
        Engine.Instance.FPS = UserSettings.Default.Debug.Framerate;

        #region Cheats
        Invincibility = UserSettings.Default.Debug.Cheat.Invincibility;
        NoDamage = UserSettings.Default.Debug.Cheat.NoDamage;
        #endregion
#endif

        #endregion

        #region Miscellaneous
        //ChangeFormLocation(settings.Miscellaneous.ScreenX_Coordinate, settings.Miscellaneous.ScreenY_Coordinate);
        #endregion

        //foreach (var c in controllers)
        //    c.LoadSettings(settings);
    }

    private void WrongConfigAlert(string message)
    {
        MessageBoxManager.GetMessageBoxStandard("Config File Invalid Value", message, ButtonEnum.Ok, Icon.Error);
    }

    private void Engine_Exception(Exception e)
    {
        MessageBoxManager.GetMessageBoxStandard("Game Error", e.Message, ButtonEnum.Ok, Icon.Error);

        CloseGame();
    }
}
