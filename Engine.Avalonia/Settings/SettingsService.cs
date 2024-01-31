using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MegaMan.Engine.Avalonia.Settings
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
                        loadedSettings = (UserSettings)serializer.Deserialize(file);
                    }
                }
                else
                {
                    loadedSettings = GetInitialSettings();
                }
            }
            catch (InvalidOperationException)
            {
                HandleInvalidConfig();
                loadedSettings = GetInitialSettings();
            }

            return loadedSettings;
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
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.Paths.AppName, Constants.Paths.SettingFile);
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

            newFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, newFileName);

            File.Move(SettingsFilePath, newFileName);
        }

        private void WrongConfigAlert(string message)
        {
            MessageBoxManager.GetMessageBoxStandard("Config File Invalid Value", message, ButtonEnum.Ok, Icon.Warning);
        }

        private UserSettings GetInitialSettings()
        {
            return new UserSettings {
                Settings = new List<Setting> { GetDefaultConfig() },
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
