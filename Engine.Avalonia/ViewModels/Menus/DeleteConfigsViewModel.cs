using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using CommunityToolkit.Mvvm.Input;
using MegaMan.Engine.Avalonia.Settings;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace MegaMan.Engine.Avalonia.ViewModels.Menus;
internal class DeleteConfigsViewModel : ViewModelBase
{
    private SettingsService settingsService;
    private UserSettings userSettings;

    public IEnumerable<ConfigName> GameItems { get => userSettings.GetAllConfigsGameNameFromCurrentUserSettings()
        .Select(name => new ConfigName(name, string.IsNullOrWhiteSpace(name) ? Constants.noGameConfigNameToDisplayToUser : name));
    }

    public ConfigName? SelectedGame { get; }

    public ICommand DeleteGameConfig { get; }
    public ICommand DeleteAllConfigs { get; }

    public DeleteConfigsViewModel()
    {
        settingsService = new SettingsService();

        var settingsPath = settingsService.SettingsFilePath;

        userSettings = settingsService.GetSettings();

        DeleteGameConfig = new RelayCommand(DeleteSelected, () => SelectedGame != null);
        DeleteAllConfigs = new RelayCommand(DeleteAll);
    }

    private async void DeleteSelected()
    {
        if (SelectedGame == null) return;

        var dialog = MessageBoxManager.GetMessageBoxStandard("C# MegaMan Engine", "Are you sure you want to delete configuration for " + SelectedGame.Value + "?", ButtonEnum.YesNo, Icon.Warning);
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            if ((await dialog.ShowAsPopupAsync(desktop.MainWindow)) == ButtonResult.No) return;    

        // Delete configuration
        userSettings.deleteSetting(SelectedGame.Value);
        XML.SaveToConfigXML(userSettings, settingsService.SettingsFilePath);

        OnPropertyChanged(nameof(GameItems));
    }

    private async void DeleteAll()
    {
        var dialog = MessageBoxManager.GetMessageBoxStandard("C# MegaMan Engine", "Are you sure you want to delete ALL configurations?", ButtonEnum.YesNo, Icon.Warning);
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            if ((await dialog.ShowAsPopupAsync(desktop.MainWindow)) == ButtonResult.No) return;

        // Delete configurations
        userSettings.deleteAllSetting();
        XML.SaveToConfigXML(userSettings, settingsService.SettingsFilePath);

        OnPropertyChanged(nameof(GameItems));
    }

    public record ConfigName(string Key, string Value);
}
