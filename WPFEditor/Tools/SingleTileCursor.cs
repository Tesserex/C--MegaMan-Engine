using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace MegaMan.Editor.Tools
{
    public class SingleTileCursor : IToolCursor
    {
        private Tileset _tileset;
        private Tile _tile;

        public SingleTileCursor(Tileset tileset, Tile tile)
        {
            _tileset = tileset;
            _tile = tile;
        }

        public ImageSource CursorImage
        {
            get { return SpriteBitmapCache.GetOrLoadFrame(_tileset.SheetPath.Absolute, _tile.Sprite.CurrentFrame.SheetLocation); }
        }

        public Double CursorWidth
        {
            get { return _tile.Width; }
        }

        public Double CursorHeight
        {
            get { return _tile.Height; }
        }
    }
}
