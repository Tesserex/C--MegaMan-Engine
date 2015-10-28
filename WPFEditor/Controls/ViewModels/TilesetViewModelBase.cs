using MegaMan.Common;
using System;
using System.Collections.Generic;
using MegaMan.Editor.Bll;
using System.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace MegaMan.Editor.Controls.ViewModels
{
    public abstract class TilesetViewModelBase : INotifyPropertyChanged
    {
        protected TilesetDocument _tileset
        {
            get; private set;
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

        public Tile SelectedTile { get; protected set; }

        public abstract void ChangeTile(Tile tile);

        protected void SetTileset(TilesetDocument tileset)
        {
            _tileset = tileset;

            if (_tileset != null)
            {
                _observedTiles = new ObservableCollection<Tile>(_tileset.Tiles);

                if (_tileset.Tiles.Any())
                    ChangeTile(_tileset.Tiles.First());
                else
                    ChangeTile(null);

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
