using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
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
        public ICommand AnimateTilesCommand { get; private set; }

        public SpriteEditorViewModel Sprite { get; private set; }

        public TilesetDocument Tileset { get { return _tileset; } }

        public string RelSheetPath
        {
            get
            {
                if (_tileset == null)
                    return null;
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
                return null;
            }
            set
            {
                foreach (var t in MultiSelectedTiles)
                    t.Properties = value;
            }
        }

        public IEnumerable<string> SelectedTileGroups
        {
            get
            {
                return MultiSelectedTiles.SelectMany(t => t.Groups).Distinct();
            }
        }

        public IEnumerable<string> AllTileGroups
        {
            get
            {
                if (_project == null)
                    return Enumerable.Empty<string>();

                return  _project.Stages
                    .Select(s => s.Tileset)
                    .SelectMany(t => t.Tiles)
                    .SelectMany(x => x.Groups)
                    .Distinct();
            }
        }

        public TileProperties EditingProperties { get; private set; }
        public Visibility ShowPropEditor { get; private set; }
        public Visibility ShowSpriteEditor { get; private set; }

        public TilesetEditorViewModel()
        {
            ChangeSheetCommand = new RelayCommand(o => ChangeSheet());
            AddTileCommand = new RelayCommand(o => AddTile());
            DeleteTileCommand = new RelayCommand(o => DeleteTile(), x => MultiSelectedTiles.Any());
            AddTilePropertiesCommand = new RelayCommand(o => AddProperties());
            EditTilePropertiesCommand = new RelayCommand(EditProperties);
            DeleteTilePropertiesCommand = new RelayCommand(DeleteProperties);
            HidePropertiesEditorCommand = new RelayCommand(o => HidePropertiesEditor());
            AnimateTilesCommand = new RelayCommand(AnimateTiles, x => MultiSelectedTiles.Any());

            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);
            ViewModelMediator.Current.GetEvent<ProjectChangedEventArgs>().Subscribe(ProjectChanged);

            ShowSpriteEditor = Visibility.Visible;
            ShowPropEditor = Visibility.Collapsed;
        }

        private void ProjectChanged(object sender, ProjectChangedEventArgs e)
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
            _project.Dirty = true;
        }

        private void DeleteTile()
        {
            foreach (var tile in MultiSelectedTiles)
            {
                _tileset.RemoveTile(tile);
                _observedTiles.Remove(tile);
            }

            ChangeTile(null);
            _project.Dirty = true;
        }

        private void AddProperties()
        {
            var properties = new TileProperties { Name = "New Properties" };
            _tileset.Tileset.AddProperties(properties);
            _observedProperties.Add(properties);
            _project.Dirty = true;
            OnPropertyChanged("TileProperties");
        }

        private void EditProperties(object obj)
        {
            EditingProperties = (TileProperties)obj;
            ShowPropEditor = Visibility.Visible;
            ShowSpriteEditor = Visibility.Collapsed;
            OnPropertyChanged("EditingProperties");
            OnPropertyChanged("ShowPropEditor");
            OnPropertyChanged("ShowSpriteEditor");
        }

        private void DeleteProperties(object obj)
        {
            var props = (TileProperties)obj;
            _tileset.Tileset.DeleteProperties(props);
            _observedProperties.Remove(props);
            _project.Dirty = true;
            OnPropertyChanged("TileProperties");
        }

        private void HidePropertiesEditor()
        {
            ShowPropEditor = Visibility.Collapsed;
            ShowSpriteEditor = Visibility.Visible;
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
            {
                var isPlaying = Sprite != null && Sprite.Sprite.Playing;
                var vm = new SpriteViewModel(tile.Sprite);
                Sprite = new SpriteEditorViewModel(vm, _project);
                if (isPlaying)
                {
                    Sprite.Sprite.Play();
                }
            }
            else
            {
                Sprite = null;
            }

            OnPropertyChanged(nameof(Sprite));
            OnPropertyChanged(nameof(SelectedTile));
            OnPropertyChanged(nameof(SelectedTileProperties));
            OnPropertyChanged(nameof(SelectedTileGroups));
        }

        private void RefreshSheet(object sender, EventArgs e)
        {
            if (Sprite != null)
                Sprite.RefreshSheet();
        }

        private void AnimateTiles(object obj)
        {
            if (MultiSelectedTiles.Count() > 1)
            {
                var first = MultiSelectedTiles.First();
                foreach (var frame in first.Sprite)
                {
                    if (frame.Duration == 0)
                        frame.Duration = 6;
                }

                foreach (var tile in MultiSelectedTiles.Skip(1))
                {
                    foreach (var frame in tile.Sprite)
                    {
                        first.Sprite.Add(frame);
                        if (frame.Duration == 0)
                            frame.Duration = 6;
                    }

                    _tileset.RemoveTile(tile);
                }
                
                ChangeTile(first);
            }
        }
    }
}
