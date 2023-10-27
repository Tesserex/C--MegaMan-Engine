﻿using System.IO;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Engine.Avalonia.ViewModels;

namespace Engine.Avalonia.Views;

public partial class MainView : UserControl
{
    private string? initialFolder;

    public MainView()
    {
        InitializeComponent();
    }

    private async void OpenGameClicked(object? sender, RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

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

    private static FilePickerFileType GameFile { get; } = new("Game File") {
        Patterns = new[] { "*.xml" },
        MimeTypes = new[] { "text/xml" }
    };
}
