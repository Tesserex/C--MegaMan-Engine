using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class TilesetEditorViewModel : TilesetViewModelBase
    {
        private ProjectDocument _project;
        private ObservableCollection<TileProperties> _observedProperties;

        public ICommand ChangeSheetCommand { get; private set; }
        public ICommand AddTileCommand { get; private set; }
        public ICommand DeleteTileCommand { get; private set; }
        public ICommand AddTilePropertiesCommand { get; private set; }
        public ICommand EditTilePropertiesCommand { get; private set; }
        public ICommand DeleteTilePropertiesCommand { get; private set; }
        public ICommand HidePropertiesEditorCommand { get; private set; }
        public ICommand ChangeTilesetCommand { get; private set; }

        public SpriteEditorViewModel Sprite { get; private set; }

        public TilesetDocument Tileset { get { return _tileset; } }

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
                    return _observedProperties;
            }
        }

        public TileProperties SelectedTileProperties
        {
            get
            {
                var props = MultiSelectedTiles.Select(t => t.Properties).Distinct().ToList();
                if (props.Count == 1)
                    return props[0];
                else
                    return null;
            }
            set
            {
                foreach (var t in MultiSelectedTiles)
                    t.Properties = value;
            }
        }

        public TileProperties EditingProperties { get; private set; }
        public System.Windows.Visibility ShowPropEditor { get; private set; }
        public System.Windows.Visibility ShowSpriteEditor { get; private set; }

        public TilesetEditorViewModel()
        {
            ChangeSheetCommand = new RelayCommand(o => ChangeSheet());
            AddTileCommand = new RelayCommand(o => AddTile());
            DeleteTileCommand = new RelayCommand(o => DeleteTile());
            AddTilePropertiesCommand = new RelayCommand(o => AddProperties());
            EditTilePropertiesCommand = new RelayCommand(EditProperties);
            DeleteTilePropertiesCommand = new RelayCommand(DeleteProperties);
            HidePropertiesEditorCommand = new RelayCommand(o => HidePropertiesEditor());
            ChangeTilesetCommand = new RelayCommand(ChangeTileset);

            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);
            ViewModelMediator.Current.GetEvent<ProjectOpenedEventArgs>().Subscribe(ProjectOpened);

            ShowSpriteEditor = System.Windows.Visibility.Visible;
            ShowPropEditor = System.Windows.Visibility.Collapsed;
        }

        private void ChangeTileset(object obj)
        {

        }

        private void ProjectOpened(object sender, ProjectOpenedEventArgs e)
        {
            _project = e.Project;
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            if (e.Stage != null)
                ChangeTileset(e.Stage.Tileset);
            else
                ChangeTileset(null);
        }

        private void ChangeTileset(TilesetDocument tileset)
        {
            if (_tileset != null)
                _tileset.TilesheetModified -= RefreshSheet;

            SetTileset(tileset);

            if (_tileset != null)
            {
                _observedProperties = new ObservableCollection<TileProperties>(_tileset.Properties);

                if (!File.Exists(_tileset.SheetPath.Absolute))
                {
                    ChangeSheet();
                }

                _tileset.TilesheetModified += RefreshSheet;
            }
            else
            {
                _observedProperties = new ObservableCollection<TileProperties>();
            }

            OnPropertyChanged("TileProperties");
            OnPropertyChanged("RelSheetPath");
            OnPropertyChanged("Tileset");
        }

        private void AddTile()
        {
            var tile = _tileset.AddTile();
            _observedTiles.Add(tile);
            ((App)App.Current).AnimateSprite(tile.Sprite);
            this._project.Dirty = true;
        }

        private void DeleteTile()
        {
            if (SelectedTile != null)
            {
                _tileset.RemoveTile(SelectedTile);
                _observedTiles.Remove(SelectedTile);
                ChangeTile(null);
                this._project.Dirty = true;
            }
        }

        private void AddProperties()
        {
            var properties = new TileProperties() { Name = "New Properties" };
            _tileset.Tileset.AddProperties(properties);
            _observedProperties.Add(properties);
            this._project.Dirty = true;
            OnPropertyChanged("TileProperties");
        }

        private void EditProperties(object obj)
        {
            EditingProperties = (TileProperties)obj;
            ShowPropEditor = System.Windows.Visibility.Visible;
            ShowSpriteEditor = System.Windows.Visibility.Collapsed;
            OnPropertyChanged("EditingProperties");
            OnPropertyChanged("ShowPropEditor");
            OnPropertyChanged("ShowSpriteEditor");
        }

        private void DeleteProperties(object obj)
        {
            var props = (TileProperties)obj;
            _tileset.Tileset.DeleteProperties(props);
            _observedProperties.Remove(props);
            this._project.Dirty = true;
            OnPropertyChanged("TileProperties");
        }

        private void HidePropertiesEditor()
        {
            ShowPropEditor = System.Windows.Visibility.Collapsed;
            ShowSpriteEditor = System.Windows.Visibility.Visible;
            OnPropertyChanged("ShowPropEditor");
            OnPropertyChanged("ShowSpriteEditor");
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
                _tileset.Tileset.ChangeSheetPath(dialog.FileName);
                OnPropertyChanged("SheetPath");
                OnPropertyChanged("RelSheetPath");

                if (_tileset.Tiles.Any())
                    ChangeTile(_tileset.Tiles.First());
            }
        }

        public override void ChangeTile(Tile tile)
        {
            base.ChangeTile(tile);

            if (tile != null)
                Sprite = new SpriteEditorViewModel(new SpriteViewModel(tile.Sprite), this._project);
            else
                Sprite = null;

            OnPropertyChanged("Sprite");
            OnPropertyChanged("SelectedTile");
            OnPropertyChanged("SelectedTileProperties");
        }

        private void RefreshSheet(object sender, System.EventArgs e)
        {
            if (Sprite != null)
                Sprite.RefreshSheet();
        }
    }
}
