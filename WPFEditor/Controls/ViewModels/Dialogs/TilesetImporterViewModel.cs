using System.Windows.Input;
using System.Windows.Media;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Algorithms;
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

        public TilesetImporterViewModel(TilesetDocument tileset)
        {
            _importer = new TilesetImporter(tileset);
            SetTileset(tileset);
            ImportImagesCommand = new RelayCommand(x => ImportImages());
            OnPropertyChanged("TilesheetSource");
            OnPropertyChanged("TilesheetWidth");
            OnPropertyChanged("TilesheetHeight");
        }

        private void ImportImages()
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

            _importer.AddImages(dialog.FileNames);

            OnPropertyChanged("TilesheetSource");
            OnPropertyChanged("TilesheetWidth");
            OnPropertyChanged("TilesheetHeight");
        }

        public override void ChangeTile(Tile tile)
        {
            
        }
    }
}
