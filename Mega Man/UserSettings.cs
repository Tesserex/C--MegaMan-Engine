using System;
using System.Windows.Forms;
using System.Collections.Generic;

namespace MegaMan.Engine
{
    #region Error Messages Constants
    #region Config Files Invalid Values
    public class ConfigFileInvalidValuesMessages
    {
        public static readonly string Size = "Size value from configuration file is invalid. Default value will be used.";
        public static readonly string NTSC_Option = "NTSC_Option value from configuration file is invalid. Default value will be used.";
        public static readonly string PixellatedOrSmoothed = "Pixellated value from configuration file is invalid. Default value will be used.";
    }
    #endregion
    #endregion

    #region Default Values
    #region Config Files Default Values
    public class ConfigFilesDefaultValues
    {
        public static readonly Int16 Size = 0;
        public static readonly Int16 NTSC_Option = 0;
        public static readonly Int16 PixellatedOrSmoothed = 0;
        public static readonly Int32 Framerate = 60;
    }
    #endregion
    #endregion

    #region Constant Values
    public class Constants
    {
        #region Paths
        public class Paths
        {
            public static readonly string SettingFile = "settings.xml";
        }
        #endregion
        #region Engine Properties
        public class EngineProperties
        {
            public static readonly Int16 FramerateMin = 1;
            public static readonly Int32 FramerateMax = 500;
        }
        #endregion
    }
    #endregion

    #region Settings Serialization Class
    [Serializable]
    public class UserSettings
    {
        public List<Setting> Settings { get; set; }

        /// <summary>
        /// Default constructor must exist to used deserialization
        /// </summary>
        public UserSettings() { }

        public UserSettings(UserKeys keys)
        {
            
        }

        public Setting GetSettingsForGame(string gameName = "")
        {
            foreach (Setting setting in Settings)
            {
                if (setting.GameFileName == gameName) return setting;
            }

            // Setting of name received not found, return default one
            foreach (Setting setting in Settings)
            {
                if (setting.GameFileName == "") return setting;
            }

            // No default settings found, return null, will caues crash and default settings will be load.
            return null;
        }

        public void AddOrSetExistingSettingsForGame(Setting newSetting, string gameName = "")
        {
            // No list, create a new one
            if (Settings == null)
            {
                Settings = new List<Setting>();
                Settings.Add(newSetting);
                return;
            }

            // If setting exist, replace it
            for (int x = 0; x < Settings.Count; x++)
            {
                if (Settings[x].GameFileName == gameName) Settings[x] = newSetting; return;
            }

            // Setting of name received not found, add it
            Settings.Add(newSetting);
        }
    }

    [Serializable]
    public class Setting
    {
        public string GameFileName { get; set; }
        public UserKeys Keys { get; set; }
        public LastScreen Screens { get; set; }
        public LastAudio Audio { get; set; }
        public LastDebug Debug { get; set; }
        public LastMiscellaneous Miscellaneous { get; set; }
    }

    [Serializable]
    public class UserKeys
    {
        public Keys Up { get; set; }
        public Keys Down { get; set; }
        public Keys Left { get; set; }
        public Keys Right { get; set; }
        public Keys Jump { get; set; }
        public Keys Shoot { get; set; }
        public Keys Start { get; set; }
        public Keys Select { get; set; }
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
    }

    [Serializable]
    public class LastScreen
    {
        public Int32 Size { get; set; } // Needs to be Int32 even if Int16 is sufficient. It is compared to Int16
        public bool Maximized { get; set; }
        public Int32 NTSC_Options { get; set; }
        public NTSC_CustomOptions NTSC_Custom { get; set; }
        public Int32 Pixellated { get; set; }
        public bool HideMenu { get; set; }
    }

    [Serializable]
    public class LastAudio
    {
        public Int32 Volume { get; set; }
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
    public class LastBackground
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
        public Int32 Framerate { get; set; }
        public LastCheat Cheat { get; set; }
        public LastBackground Layers { get; set; }
    }

    [Serializable]
    public class LastMiscellaneous
    {
        public int ScreenX_Coordinate { get; set; }
        public int ScreenY_Coordinate { get; set; }
    }
    #endregion
}
