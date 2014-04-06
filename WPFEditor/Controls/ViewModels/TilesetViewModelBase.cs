using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.Editor.Controls.ViewModels
{
    public abstract class TilesetViewModelBase
    {
        protected Tileset _tileset;

        public string SheetPath
        {
            get
            {
                if (_tileset != null)
                {
                    return _tileset.SheetPath.Absolute;
                }
                else
                {
                    return null;
                }
            }
        }

        public virtual IEnumerable<Tile> Tiles
        {
            get
            {
                return _tileset;
            }
        }

        public Tile SelectedTile { get; protected set; }

        public abstract void ChangeTile(Tile tile);
    }
}
