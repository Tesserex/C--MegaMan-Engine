using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Common
{
    public class TileSprite : Sprite
    {
        private Tileset _tileset;

        public TileSprite(Tileset tileset) : base(tileset.TileSize, tileset.TileSize)
        {
            _tileset = tileset;
        }

        public TileSprite(Tileset tileset, Sprite copy)
            : base(copy)
        {
            _tileset = tileset;
        }

        public override FilePath SheetPath
        {
            get
            {
                return _tileset.SheetPath;
            }
            set
            {
                _tileset.SheetPath = value;
            }
        }

        public override int Width
        {
            get
            {
                return _tileset.TileSize;
            }
        }

        public override int Height
        {
            get
            {
                return _tileset.TileSize;
            }
        }
    }
}
