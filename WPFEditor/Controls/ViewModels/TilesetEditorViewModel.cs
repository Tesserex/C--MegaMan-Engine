using MegaMan.Common;
using MegaMan.Editor.Bll;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class TilesetEditorViewModel : TilesetViewModelBase, INotifyPropertyChanged
    {
        private ProjectDocument _project;
        private ObservableCollection<Tile> _observedTiles;
        private ObservableCollection<TileProperties> _observedProperties;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public ICommand ChangeSheetCommand { get; private set; }
        public ICommand AddTileCommand { get; private set; }

        public SpriteEditorViewModel Sprite { get; private set; }

        public string RelSheetPath
        {
            get
            {
                return _tileset.SheetPath.Relative;
            }
        }

        public override IEnumerable<Tile> Tiles
        {
            get
            {
                return _observedTiles;
            }
        }

        public IEnumerable<TileProperties> TileProperties
        {
            get
            {
                return _tileset.Properties;
            }
        }

        public TilesetEditorViewModel(Tileset tileset, ProjectDocument project)
        {
            _tileset = tileset;
            _project = project;
            _observedTiles = new ObservableCollection<Tile>(_tileset);
            _observedProperties = new ObservableCollection<TileProperties>(_tileset.Properties);

            if (_tileset.Any())
                ChangeTile(_tileset.First());

            ChangeSheetCommand = new RelayCommand(o => ChangeSheet());
            AddTileCommand = new RelayCommand(o => AddTile());

            if (!File.Exists(_tileset.SheetPath.Absolute))
            {
                ChangeSheet();
            }

            ((App)App.Current).AnimateTileset(_tileset);
        }

        private void AddTile()
        {
            var tile = _tileset.AddTile();
            _observedTiles.Add(tile);
            ((App)App.Current).AnimateSprite(tile.Sprite);
        }

        private void ChangeSheet()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Images", "png,gif,jpg,jpeg,bmp"));

            if (_tileset.SheetPath != null)
                dialog.InitialDirectory = _tileset.SheetPath.Absolute;
            else
                dialog.InitialDirectory = _project.Project.BaseDir;

            dialog.Title = "Choose Tileset Image";
            dialog.EnsureFileExists = true;
            dialog.EnsurePathExists = true;
            dialog.EnsureReadOnly = false;
            dialog.EnsureValidNames = true;
            dialog.Multiselect = false;
            dialog.ShowPlacesList = true;

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                _tileset.ChangeSheetPath(dialog.FileName);
                OnPropertyChanged("SheetPath");
                OnPropertyChanged("RelSheetPath");

                if (_tileset.Any())
                    ChangeTile(_tileset.First());
            }
        }

        public override void ChangeTile(Tile tile)
        {
            SelectedTile = tile;

            Sprite = new SpriteEditorViewModel(tile.Sprite);
            OnPropertyChanged("Sprite");

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedTile"));
            }
        }
    }
}
