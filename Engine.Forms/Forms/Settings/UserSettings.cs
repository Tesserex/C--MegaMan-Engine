using System.Xml;
using System.Xml.Serialization;
using MegaMan.Engine.Input;
using SharpDX.DirectInput;
using SharpDX.XInput;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace MegaMan.Engine.Forms.Settings
{
    #region Error Messages Constants
    #region Config Files Invalid Values
    public class ConfigFileInvalidValuesMessages
    {
        public static readonly string Size = "Size value from configuration file is invalid. Default value will be used.";
        public static readonly string NTSC_Option = "NTSC_Option value from configuration file is invalid. Default value will be used.";
        public static readonly string PixellatedOrSmoothed = "Pixellated value from configuration file is invalid. Default value will be used.";
        public static readonly string CannotDeserializeXML = "Cannot deserialize config file. File renamed to {0}.";
    }
    #endregion
    #endregion

    #region Constant Values
    public class Constants
    {
        #region Errors
        public class Errors
        {
            public static readonly Int16 GetUserSettingsFromXML_NoError = 0;
            public static readonly Int16 GetUserSettingsFromXML_FileNotFound = 1;
            public static readonly Int16 GetUserSettingsFromXML_CannotDeserialize = 2;

            public static readonly Int16 LoadConfigFromXML_NoError = 0;
            public static readonly Int16 LoadConfigFromXML_FileNotFound = 1;
            public static readonly Int16 LoadConfigFromXML_CannotDeserialize = 2;
            public static readonly Int16 LoadConfigFromXML_NoContentReadFromXML = 3;
            public static readonly Int16 LoadConfigFromXML_NoDefaultValueInXML = 4;
        }
        #endregion
        #region Paths
        public class Paths
        {
            public static readonly string SettingFile = "settings.xml";
        }
        #endregion
        
        public static readonly string noGameConfigNameToDisplayToUser = "No Game";
        public static readonly string settingNameForFactorySettings = "Default Settings";
    }
    #endregion

    public static class XML
    {
        public static void SaveToConfigXML(UserSettings userSettings, string settingsPath, string fileName = null)
        {
            if (fileName == null) fileName = Constants.Paths.SettingFile;

            var serializer = new XmlSerializer(typeof(UserSettings));

            var writer = new XmlTextWriter(settingsPath, null)
            {
                Indentation = 1,
                IndentChar = '\t',
                Formatting = Formatting.Indented
            };

            serializer.Serialize(writer, userSettings);

            writer.Close();
        }
    }

    #region Settings Serialization Class
    [Serializable]
    public class UserSettings
    {
        public bool AutosaveSettings { get; set; }
        public bool UseDefaultSettings { get; set; }
        public string Autoload { get; set; } // Game with his path to load on startup
        public string InitialFolder { get; set; } // Always remember last place navigating with open folder.
        public List<RecentGame> RecentGames { get; set; } 
        public List<Setting> Settings { get; set; }

        public void deleteSetting(int index)
        {
            try
            {
                Settings.RemoveAt(index);
            }
            catch (Exception)
            {
            }
        }

        public void deleteAllSetting()
        {
            try
            {
                Settings = null;
            }
            catch (Exception)
            {
            }
        }

        public void AddRecentGame(string name, string path)
        {
            if (RecentGames == null)
            {
                RecentGames = new List<RecentGame>();
            }

            var existing = RecentGames.FirstOrDefault(x => x.Path == path);
            if (existing == null)
            {
                existing = new RecentGame { Name = name, Path = path };
            }
            else
            {
                RecentGames.Remove(existing);
            }

            RecentGames.Insert(0, existing);
        }

        public Setting GetSettingByIndex(int index)
        {
            try
            {
                return Settings[index];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Setting GetSettingsForGame(string gameName = "")
        {
            foreach (var setting in Settings)
            {
                if (setting.GameFileName == gameName) return setting;
            }

            // Setting of name received not found, return default one
            foreach (var setting in Settings)
            {
                if (setting.GameFileName == "") return setting;
            }

            // No default settings found, return null.
            return null;
        }

        public void AddOrSetExistingSettingsForGame(Setting newSetting)
        {
            // No list, create a new one
            if (Settings == null)
            {
                Settings = new List<Setting>();
                Settings.Add(newSetting);
                return;
            }

            // If setting exist, replace it
            for (var x = 0; x < Settings.Count; x++)
            {
                if (Settings[x].GameFileName == newSetting.GameFileName)
                {
                    Settings[x] = newSetting; return;
                }
            }

            // Setting of name received not found, add it
            Settings.Add(newSetting);
        }

        public List<string> GetAllConfigsGameNameFromCurrentUserSettings()
        {
            if (Settings != null)
                return Settings.Select(s => s.GameTitle).ToList();
            return null;
        }

        public static Setting Default { get; private set; }

        static UserSettings()
        {
            Default = new Setting {
                GameFileName = "",
                KeyBindings = new List<UserKeyBindingSetting> {
                    new UserKeyBindingSetting { Input = GameInputs.Up, Key = Keys.Up },
                    new UserKeyBindingSetting { Input = GameInputs.Down, Key = Keys.Down },
                    new UserKeyBindingSetting { Input = GameInputs.Left, Key = Keys.Left },
                    new UserKeyBindingSetting { Input = GameInputs.Right, Key = Keys.Right },
                    new UserKeyBindingSetting { Input = GameInputs.Jump, Key = Keys.A },
                    new UserKeyBindingSetting { Input = GameInputs.Shoot, Key = Keys.S },
                    new UserKeyBindingSetting { Input = GameInputs.Start, Key = Keys.Enter },
                    new UserKeyBindingSetting { Input = GameInputs.Select, Key = Keys.Space }
                },
                Screens = new LastScreen {
                    Size = ScreenScale.X1,
                    Maximized = false,
                    HideMenu = false,
                    Pixellated = PixellatedOrSmoothed.Pixellated,
                    NTSC_Options = NTSC_Options.None,
                    NTSC_Custom = new NTSC_CustomOptions {
                        Hue = 0,
                        Saturation = 0,
                        Brightness = 0,
                        Contrast = 0,
                        Sharpness = 0,
                        Gamma = 0,
                        Resolution = 0,
                        Artifacts = 0,
                        Fringing = 0,
                        Bleed = 0,
                        Merge_Fields = true
                    }
                },
                Audio = new LastAudio {
                    Volume = 50,
                    Musics = true,
                    Sound = true,
                    Square1 = true,
                    Square2 = true,
                    Triangle = true,
                    Noise = true
                },
                Debug = new LastDebug {
                    ShowMenu = true,
                    ShowHitboxes = false,
                    Framerate = 60,
                    Layers = new LastLayers {
                        Background = true,
                        Sprites1 = true,
                        Sprites2 = true,
                        Sprites3 = true,
                        Sprites4 = true,
                        Foreground = true
                    },
                    Cheat = new LastCheat {
                        Invincibility = false,
                        NoDamage = false
                    }
                },
                Miscellaneous = new LastMiscellaneous {
                    ScreenX_Coordinate = -1,        // -1 means centered
                    ScreenY_Coordinate = -1
                }
            };
        }
    }

    [Serializable]
    public class Setting
    {
        public string GameFileName { get; set; }
        public string GameTitle { get; set; }
        public List<UserKeyBindingSetting> KeyBindings { get; set; }
        public List<UserJoystickBindingSetting> JoystickBindings { get; set; }
        public List<UserGamepadBindingSetting> GamepadBindings { get; set; }
        public InputTypes ActiveInput { get; set; }
        public LastScreen Screens { get; set; }
        public LastAudio Audio { get; set; }
        public LastDebug Debug { get; set; }
        public LastMiscellaneous Miscellaneous { get; set; }

        public Setting()
        {
            KeyBindings = new List<UserKeyBindingSetting>();
            JoystickBindings = new List<UserJoystickBindingSetting>();
            GamepadBindings = new List<UserGamepadBindingSetting>();
            ActiveInput = InputTypes.Keyboard;
            Screens = new LastScreen();
            Audio = new LastAudio();
            Debug = new LastDebug();
            Miscellaneous = new LastMiscellaneous();
        }
    }
    
    public interface IUserInputBindingSetting
    {
        GameInputs Input { get; set; }
        IGameInputBinding GetGameInputBinding();
    }

    [Serializable]
    public class UserKeyBindingSetting : IUserInputBindingSetting
    {
        public GameInputs Input { get; set; }
        public Keys Key { get; set; }

        public IGameInputBinding GetGameInputBinding()
        {
            return new KeyboardInputBinding(Input, Key, k => Program.KeyDown((System.Windows.Forms.Keys)(int)k));
        }
    }

    [Serializable]
    public class UserJoystickBindingSetting : IUserInputBindingSetting
    {
        public GameInputs Input { get; set; }
        public Guid DeviceGuid { get; set; }
        public JoystickOffset Button { get; set; }
        public int Value { get; set; }

        public IGameInputBinding GetGameInputBinding()
        {
            return new JoystickInputBinding(Input, DeviceGuid, Button, Value);
        }
    }

    [Serializable]
    public class UserGamepadBindingSetting : IUserInputBindingSetting
    {
        public GameInputs Input { get; set; }
        public GamepadButtonFlags Button { get; set; }

        public IGameInputBinding GetGameInputBinding()
        {
            return new GamepadInputBinding(Input, Button);
        }
    }

    [Serializable]
    public class NTSC_CustomOptions
    {
        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Brightness { get; set; }
        public double Contrast { get; set; }
        public double Sharpness { get; set; }
        public double Gamma { get; set; }
        public double Resolution { get; set; }
        public double Artifacts { get; set; }
        public double Fringing { get; set; }
        public double Bleed { get; set; }
        public bool Merge_Fields { get; set; }
    }

    [Serializable]
    public class LastScreen
    {
        public ScreenScale Size { get; set; }
        public bool Maximized { get; set; }
        public NTSC_Options NTSC_Options { get; set; }
        public NTSC_CustomOptions NTSC_Custom { get; set; }
        public PixellatedOrSmoothed Pixellated { get; set; }
        public bool HideMenu { get; set; }

        public LastScreen()
        {
            NTSC_Custom = new NTSC_CustomOptions();
        }
    }

    [Serializable]
    public class LastAudio
    {
        public int Volume { get; set; }
        public bool Musics { get; set; }
        public bool Sound { get; set; }
        public bool Square1 { get; set; }
        public bool Square2 { get; set; }
        public bool Triangle { get; set; }
        public bool Noise { get; set; }
    }

    [Serializable]
    public class LastCheat
    {
        public bool Invincibility { get; set; }
        public bool NoDamage { get; set; }
    }

    [Serializable]
    public class LastLayers
    {
        public bool Background { get; set; }
        public bool Sprites1 { get; set; }
        public bool Sprites2 { get; set; }
        public bool Sprites3 { get; set; }
        public bool Sprites4 { get; set; }
        public bool Foreground { get; set; }
    }

    [Serializable]
    public class LastDebug
    {
        public bool ShowMenu { get; set; }
        public bool ShowHitboxes { get; set; }
        public int Framerate { get; set; }
        public LastCheat Cheat { get; set; }
        public LastLayers Layers { get; set; }

        public LastDebug()
        {
            Cheat = new LastCheat();
            Layers = new LastLayers();
        }
    }

    [Serializable]
    public class LastMiscellaneous
    {
        public int ScreenX_Coordinate { get; set; }
        public int ScreenY_Coordinate { get; set; }
    }

    [Serializable]
    public class RecentGame
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }
    #endregion
}
