using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using MegaMan.Common;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls.ViewModels
{
    public abstract class TilesetViewModelBase : INotifyPropertyChanged
    {
        protected TilesetDocument _tileset
        {
            get;
            private set;
        }

        protected ObservableCollection<Tile> _observedTiles;

        public string SheetPath
        {
            get
            {
                return _tileset.SheetPath != null ? _tileset.SheetPath.Absolute : null;
            }
        }

        public virtual IEnumerable<Tile> Tiles
        {
            get
            {
                return _observedTiles;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Tile _selectedTile;

        public Tile SelectedTile
        {
            get { return _selectedTile; }
            set
            {
                ChangeTile(value);
            }
        }

        private List<Tile> _multiSelectedTiles;

        public IEnumerable<Tile> MultiSelectedTiles
        {
            get { return _multiSelectedTiles.AsReadOnly(); }
            set
            {
                _multiSelectedTiles = value.ToList();
                OnPropertyChanged("MultiSelectedTiles");
            }
        }

        public TilesetViewModelBase()
        {
            _multiSelectedTiles = new List<Tile>();

            ViewModelMediator.Current.GetEvent<StageChangedEventArgs>().Subscribe(StageChanged);
            ViewModelMediator.Current.GetEvent<TileSelectedEventArgs>().Subscribe((s, e) => ChangeTile(e.Tile));
        }

        public virtual void ChangeTile(Tile tile)
        {
            _selectedTile = tile;
            OnPropertyChanged("SelectedTile");
        }

        protected virtual void SetTileset(TilesetDocument tileset)
        {
            if (_tileset != null)
                _tileset.TilesetModified -= Update;

            _tileset = tileset;

            if (_tileset != null)
            {
                _observedTiles = new ObservableCollection<Tile>(_tileset.Tiles);

                if (_tileset.Tiles.Any())
                    ChangeTile(_tileset.Tiles.First());
                else
                    ChangeTile(null);

                _tileset.TilesetModified += Update;
                ((App)App.Current).AnimateTileset(_tileset.Tileset);
            }
            else
            {
                _observedTiles = new ObservableCollection<Tile>();
                ChangeTile(null);
            }

            OnPropertyChanged("Tiles");
            OnPropertyChanged("SheetPath");
        }

        private void Update(object sender, EventArgs e)
        {
            // easier to reassign the whole thing than correctly update the collection
            // wrong I know but don't really care right now.
            _observedTiles = new ObservableCollection<Tile>(_tileset.Tiles);
            OnPropertyChanged("Tiles");
        }

        private void StageChanged(object sender, StageChangedEventArgs e)
        {
            if (e.Stage != null)
                SetStage(e.Stage);
            else
                UnsetStage();
        }

        private void SetStage(StageDocument stage)
        {
            SetTileset(stage.Tileset);

            ChangeTile(_tileset.Tiles.FirstOrDefault());

            OnPropertyChanged("Tiles");
        }

        private void UnsetStage()
        {
            SetTileset(null);

            ChangeTile(null);

            OnPropertyChanged("Tiles");
        }

        protected void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }
    }
}
