using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.Editor.Controls.ViewModels
{
    public class TilesetEditorViewModel : TilesetViewModelBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(property));
            }
        }

        public SpriteEditorViewModel Sprite { get; private set; }

        public TilesetEditorViewModel(Tileset tileset)
        {
            _tileset = tileset;

            Sprite = new SpriteEditorViewModel(_tileset.First().Sprite);
            OnPropertyChanged("Sprite");
        }

        public override void ChangeTile(Tile tile)
        {
            SelectedTile = tile;
            Sprite.ChangeSprite(tile.Sprite);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedTile"));
            }
        }
    }
}
