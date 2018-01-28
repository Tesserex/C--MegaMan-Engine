using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace MegaMan.Engine.Forms.Settings
{
    public class SettingsService
    {
        private UserSettings loadedSettings;

        public UserSettings GetSettings()
        {
            try
            {
                if (File.Exists(SettingsFilePath))
                {
                    var serializer = new XmlSerializer(typeof(UserSettings));
                    using (var file = File.Open(SettingsFilePath, FileMode.Open))
                    {
                        this.loadedSettings = (UserSettings)serializer.Deserialize(file);
                    }
                }
                else
                {
                    this.loadedSettings = GetInitialSettings();
                }
            }
            catch (InvalidOperationException)
            {
                HandleInvalidConfig();
                this.loadedSettings = GetInitialSettings();
            }

            return this.loadedSettings;
        }

        public Setting GetConfigForGame(string game)
        {
            var settings = GetSettings();
            var gameConfig = settings.GetSettingsForGame(game) ?? GetDefaultConfig();
            return gameConfig;
        }

        public string SettingsFilePath
        {
            get
            {
                return Path.Combine(Application.UserAppDataPath, Constants.Paths.SettingFile);
            }
        }

        public string GetAutoLoadGame()
        {
            return GetSettings().Autoload;
        }

        private void HandleInvalidConfig()
        {
            var newFileName = string.Format("bad-config-{0}.xml", DateTime.Now.ToString("yyyyMMdd-HHmmss"));

            WrongConfigAlert(string.Format(ConfigFileInvalidValuesMessages.CannotDeserializeXML, newFileName));

            newFileName = Path.Combine(Application.StartupPath, newFileName);

            File.Move(SettingsFilePath, newFileName);
        }

        private void WrongConfigAlert(string message)
        {
            MessageBox.Show(message, "Config File Invalid Value", MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
        }

        private UserSettings GetInitialSettings()
        {
            return new UserSettings() {
                Settings = new List<Setting>() { GetDefaultConfig() },
                Autoload = null,
                AutosaveSettings = true,
                InitialFolder = null,
                UseDefaultSettings = true
            };
        }

        private Setting GetDefaultConfig()
        {
            return UserSettings.Default;
        }
    }
}
