using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Algorithms;
using MegaMan.Editor.Controls.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MegaMan.Editor.Controls.ViewModels.Dialogs
{
    public class TilesetImporterViewModel : TilesetViewModelBase
    {
        private TilesetImporter _importer;

        public ImageSource TilesheetSource
        {
            get { return _importer.Tilesheet; }
        }

        public double TilesheetWidth { get { return _importer.Tilesheet != null ? _importer.Tilesheet.PixelWidth : 0; } }
        public double TilesheetHeight { get { return _importer.Tilesheet != null ? _importer.Tilesheet.PixelHeight : 0; } }

        public ICommand ImportImagesCommand { get; private set; }
        public ICommand ExtractCommand { get; private set; }
        public ICommand CompactSheetCommand { get; private set; }

        public TilesetImporterViewModel(TilesetDocument tileset)
        {
            _importer = new TilesetImporter(tileset);
            SetTileset(tileset);
            ImportImagesCommand = new RelayCommand(x => ImportImages());
            ExtractCommand = new RelayCommand(x => ExtractImages());
            CompactSheetCommand = new RelayCommand(x => CompactSheet());

            RefreshSheet();
        }

        private void RefreshSheet()
        {
            OnPropertyChanged("TilesheetSource");
            OnPropertyChanged("TilesheetWidth");
            OnPropertyChanged("TilesheetHeight");
        }

        private async void ImportImages()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Images", "png,gif,jpg,jpeg,bmp"));

            dialog.Title = "Select Images";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = true;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() != CommonFileDialogResult.Ok)
                return;

            var images = dialog.FileNames
                .Where(p => System.IO.File.Exists(p))
                .Select(p => PromptImage(new BitmapImage(new Uri(p)), p))
                .Where(m => m != null)
                .ToList();
            
            var reporter = new Progress<ProgressDialogState>();
            var stopwatch = new Stopwatch();
            var progress = ProgressDialog.Open(reporter, stopwatch);

            stopwatch.Start();
            await _importer.AddImagesAsync(images, reporter);
            stopwatch.Stop();
            progress.Close();

            RefreshSheet();
        }

        private TilesetImageImportDialogViewModel PromptImage(BitmapSource image, string path)
        {
            var boxModel = new TilesetImageImportDialogViewModel(path);
            var box = new TilesetImageImportDialog();
            box.DataContext = boxModel;
            box.ShowDialog();

            if (box.Result == System.Windows.MessageBoxResult.OK)
                return boxModel;
            else
                return null;
        }

        private void ExtractImages()
        {
            _importer.ExtractTiles();
            RefreshSheet();
        }

        private void CompactSheet()
        {
            _importer.CompactTilesheet();
            RefreshSheet();
        }

        protected override void SetTileset(TilesetDocument tileset)
        {
            base.SetTileset(tileset);
            _importer = new TilesetImporter(tileset);
            RefreshSheet();
        }

        public override void ChangeTile(Tile tile)
        {
            // NOP   
        }
    }
}
