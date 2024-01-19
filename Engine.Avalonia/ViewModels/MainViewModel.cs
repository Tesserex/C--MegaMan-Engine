using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.Xml;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Layout;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.Input;
using MegaMan.Engine.Avalonia.Settings;
using MegaMan.Engine.Avalonia.ViewModels.Menus;
using MegaMan.Engine.Input;
using MegaMan.IO.Xml;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;
using AvaloniaKeyboardInputBinding = MegaMan.Engine.Avalonia.Settings.AvaloniaKeyboardInputBinding;

namespace MegaMan.Engine.Avalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    public EngineGame? CurrentGame { get; set; }

    private bool showDebugBar;

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

    private readonly SettingsService settingsService;
    private List<IMenuViewModel> menuViewModels = new List<IMenuViewModel>();

    public bool ShowDebugBar
    {
        get => showDebugBar;
        set { showDebugBar = value; OnPropertyChanged(); }
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

    private double screenWidth, screenHeight;
    public double ScreenWidth { get => screenWidth; set { SetProperty(ref screenWidth, value); } }
    public double ScreenHeight { get => screenHeight; set { SetProperty(ref screenHeight, value); } }

    private SizeToContent sizeMode = SizeToContent.WidthAndHeight;
    public SizeToContent SizeMode { get => sizeMode; set { SetProperty(ref sizeMode, value); } }

    private HorizontalAlignment hAlign;
    private VerticalAlignment vAlign;
    public HorizontalAlignment HAlignment { get => hAlign; set { SetProperty(ref hAlign, value); } }
    public VerticalAlignment VAlignment { get => vAlign; set { SetProperty(ref vAlign, value); } }

    public ICommand ResetGameCommand { get; }
    public ICommand CloseGameCommand { get; }
    public ICommand QuitCommand { get; }
    public ICommand PauseCommand { get; }
    public ICommand OpenRecentCommand { get; }
    public ICommand AutosaveCommand { get; }
    public ICommand AutoloadCommand { get; }

    private string CurrentGamePath
    {
        get { return Game.CurrentGame != null ? Game.CurrentGame.BasePath : string.Empty; }
    }

    private string CurrentGameTitle
    {
        get { return Game.CurrentGame != null ? Game.CurrentGame.Name : string.Empty; }
    }

    internal AudioMenuViewModel AudioMenu { get; }
    internal ScreenMenuViewModel ScreenMenu { get; }
    internal CustomNtscViewModel NtscMenu { get; }

    public MainViewModel()
    {
        if (!Design.IsDesignMode)
        {
            CurrentGame = new EngineGame();
        }

        settingsService = new SettingsService();

        AudioMenu = new AudioMenuViewModel();
        menuViewModels.Add(AudioMenu);
        ScreenMenu = new ScreenMenuViewModel();
        menuViewModels.Add(ScreenMenu);
        NtscMenu = new CustomNtscViewModel();
        menuViewModels.Add(NtscMenu);
        NtscMenu.NtscOptionsChanged += () => {
            ScreenMenu.UseCustomNtsc();
        };

#if DEBUG
        ShowDebugBar = true;
#endif

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

        ScreenMenu.PropertyChanged += ScreenMenu_PropertyChanged;

        ForceSize(256, 224);
    }

    private void ScreenMenu_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ScreenMenu.Width) || e.PropertyName == nameof(ScreenMenu.Height))
        {
            ForceSize(ScreenMenu.Width, ScreenMenu.Height);
        }
    }

    public void ForceSize(double width, double height)
    {
        HAlignment = HorizontalAlignment.Center;
        VAlignment = VerticalAlignment.Center;
        ScreenWidth = width;
        ScreenHeight = height;
        SizeMode = SizeToContent.WidthAndHeight;
    }

    public void ManualSize()
    {
        // sizeMode will automatically change to Manual when the user resizes the window
        HAlignment = HorizontalAlignment.Stretch;
        VAlignment = VerticalAlignment.Stretch;
        ScreenWidth = 0;
        ScreenHeight = 0;
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

    public void Quit()
    {
        CloseApp();
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                desktop.TryShutdown();
            } catch {
                // already shutting down
            }
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
        SaveGlobalConfigValues();
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

    public void AutosaveConfig(string? fileName = null)
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
                    //HideMenu = hideMenuItem.Checked
                },
                Audio = new LastAudio(),
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

            foreach (var c in menuViewModels)
                c.SaveSettings(settings);
        }

        var userSettings = settingsService.GetSettings();
        userSettings.AddOrSetExistingSettingsForGame(settings);

        XML.SaveToConfigXML(userSettings, settingsService.SettingsFilePath, fileName);
    }

    private List<UserKeyBindingSetting> GetKeyBindingSettings()
    {
        return GameInput.GetBindingsOf<AvaloniaKeyboardInputBinding>().Select(x => new UserKeyBindingSetting { Input = x.Input, Key = x.Key }).ToList();
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

        foreach (var c in menuViewModels)
            c.LoadSettings(settings);
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

    internal void SetCustomNtsc()
    {
        ScreenMenu.UseCustomNtsc();
    }
}
