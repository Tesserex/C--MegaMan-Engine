using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MegaMan.Engine.Avalonia.ViewModels;

namespace MegaMan.Engine.Avalonia.Views;

public partial class MainView : UserControl
{
    private InputBindings? InputBindingsWindow;
    private CustomNtsc? CustomNtscWindow;

    public MainView()
    {
        InitializeComponent();
    }

    private async void OpenGameClicked(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var initialFolder = (DataContext as MainViewModel)?.InitialFolder;

        // Start async operation to open the dialog.
        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions {
            Title = "Open Game XML File",
            AllowMultiple = false,
            FileTypeFilter = new [] { GameFile },
            SuggestedStartLocation = initialFolder != null ? await topLevel.StorageProvider.TryGetFolderFromPathAsync(initialFolder) : null
        });

        if (files.Count > 0 && files[0] is not null)
        {
            var filename = files[0].TryGetLocalPath();
            if (filename == null) return;
            initialFolder = Path.GetDirectoryName(filename);

            if (DataContext is MainViewModel viewModel)
            {
                viewModel.LoadFromOpenDialog(filename);
            }
        }
    }

    private void OpenInputBindings(object? sender, RoutedEventArgs e)
    {
        if (InputBindingsWindow is null)
        {
            InputBindingsWindow = new InputBindings();
            InputBindingsWindow.Closed += (s, e) => {
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.AutosaveConfig();
                }
            };
        }

        InputBindingsWindow.Show();
    }

    private void OpenCustomNtsc(object? sender, RoutedEventArgs e)
    {
        (DataContext as MainViewModel)?.SetCustomNtsc();
        if (CustomNtscWindow is null)
        {
            CustomNtscWindow = new CustomNtsc() { DataContext = (DataContext as MainViewModel)?.NtscMenu };
            CustomNtscWindow.Closed += (s, e) => {
                if (DataContext is MainViewModel viewModel)
                {
                    viewModel.AutosaveConfig();
                }
            };
        }

        CustomNtscWindow.Show();
    }

    private void CaptureScreenClicked(object? sender, RoutedEventArgs e)
    {
        var capDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "screenshots");
        if (!Directory.Exists(capDir)) Directory.CreateDirectory(capDir);

        string capPath;
        var capNum = 1;

        do
        {
            capPath = Path.Combine(capDir, string.Format("{0}.png", capNum));
            capNum++;
        } while (File.Exists(capPath));

        using (var stream = File.OpenWrite(capPath))
        {
            gameControl.SaveCap(stream);
        }
    }

    private static FilePickerFileType GameFile { get; } = new("Game File") {
        Patterns = new[] { "*.xml" },
        MimeTypes = new[] { "text/xml" }
    };
}
