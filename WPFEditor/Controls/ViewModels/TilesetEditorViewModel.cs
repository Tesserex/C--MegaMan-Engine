using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;
using Microsoft.WindowsAPICodePack.Dialogs;

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
                if (_tileset == null)
                    return null;
                else
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
                if (_tileset == null)
                    return Enumerable.Empty<TileProperties>();
                else
                    return _tileset.Properties;
            }
        }

        public TilesetEditorViewModel()
        {
            ChangeSheetCommand = new RelayCommand(o => ChangeSheet());
            AddTileCommand = new RelayCommand(o => AddTile());

            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);
            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(ProjectOpened);
        }

        private void ProjectOpened(object sender, ProjectOpenedEventArgs e)
        {
            _project = e.Project;
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            if (e.Stage != null)
                SetTileset(e.Stage.Tileset);
            else
                SetTileset(null);
        }

        private void SetTileset(Tileset tileset)
        {
            _tileset = tileset;

            if (_tileset != null)
            {
                _observedTiles = new ObservableCollection<Tile>(_tileset);
                _observedProperties = new ObservableCollection<TileProperties>(_tileset.Properties);

                if (_tileset.Any())
                    ChangeTile(_tileset.First());
                else
                    ChangeTile(null);

                if (!File.Exists(_tileset.SheetPath.Absolute))
                {
                    ChangeSheet();
                }

                ((App)App.Current).AnimateTileset(_tileset);
            }
            else
            {
                _observedTiles = new ObservableCollection<Tile>();
                _observedProperties = new ObservableCollection<TileProperties>();
                ChangeTile(null);
            }

            OnPropertyChanged("Tiles");
            OnPropertyChanged("TileProperties");
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

            if (tile != null)
                Sprite = new SpriteEditorViewModel(tile.Sprite);
            else
                Sprite = null;

            OnPropertyChanged("Sprite");

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedTile"));
            }
        }
    }
}
