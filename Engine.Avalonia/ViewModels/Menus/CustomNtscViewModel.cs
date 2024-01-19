using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using MegaMan.Engine.Avalonia.Settings;

namespace MegaMan.Engine.Avalonia.ViewModels.Menus;
public class CustomNtscViewModel : ViewModelBase, IMenuViewModel
{
    private NTSC_CustomOptions options = new NTSC_CustomOptions();
    public event Action? NtscOptionsChanged;

    public double Hue { get => options.Hue * 180; set { options.Hue = value / 180; OnPropertyChanged(); NtscOptionsChanged?.Invoke(); } }

    public double Saturation { get => options.Saturation * 100; set { options.Saturation = value / 100; OnPropertyChanged(); NtscOptionsChanged?.Invoke(); } }

    public double Brightness { get => options.Brightness * 100; set { options.Brightness = value / 100; OnPropertyChanged(); NtscOptionsChanged?.Invoke(); } }

    public double Contrast { get => options.Contrast * 100; set { options.Contrast = value / 100; OnPropertyChanged(); NtscOptionsChanged?.Invoke(); } }

    public double Sharpness { get => options.Sharpness * 100; set { options.Sharpness = value / 100; OnPropertyChanged(); NtscOptionsChanged?.Invoke(); } }

    public double Gamma { get => options.Gamma * 100; set { options.Gamma = value / 100; OnPropertyChanged(); NtscOptionsChanged?.Invoke(); } }

    public double Resolution { get => options.Resolution * 100; set { options.Resolution = value / 100; OnPropertyChanged(); NtscOptionsChanged?.Invoke(); } }

    public double Artifacts { get => options.Artifacts * 100; set { options.Artifacts = value / 100; OnPropertyChanged(); NtscOptionsChanged?.Invoke(); } }

    public double Fringing { get => options.Fringing * 100; set { options.Fringing = value / 100; OnPropertyChanged(); NtscOptionsChanged?.Invoke(); } }

    public double Bleed { get => options.Bleed * 100; set { options.Bleed = value / 100; OnPropertyChanged(); NtscOptionsChanged?.Invoke(); } }

    public ICommand ResetCommand { get; private init; }

    public CustomNtscViewModel()
    {
        ResetCommand = new RelayCommand(Reset);
    }

    public void LoadSettings(Setting settings)
    {
        options = settings.Screens.NTSC_Custom;
    }

    public void SaveSettings(Setting settings)
    {
        settings.Screens.NTSC_Custom = options;
    }

    private void Reset()
    {
        Hue = Saturation = Brightness = Contrast = Sharpness = Gamma = Resolution = Artifacts = Fringing = Bleed = 0;
    }
}
