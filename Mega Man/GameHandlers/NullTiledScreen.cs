using System.Collections.Generic;
using MegaMan.Common;

namespace MegaMan.Engine
{
    /// <summary>
    /// This is used for scenes and menus that don't get their entity container from gameplay
    /// </summary>
    public class NullTiledScreen : ITiledScreen
    {
        public int TileSize
        {
            get
            {
                return 32;
            }
        }

        public float OffsetX
        {
            get { return 0; }
        }

        public float OffsetY
        {
            get { return 0; }
        }

        public MapSquare SquareAt(float px, float py)
        {
            return null;
        }

        public Tile TileAt(float px, float py)
        {
            return null;
        }

        public IEnumerable<MapSquare> Tiles
        {
            get { return null; }
        }

        public bool IsOnScreen(float x, float y)
        {
            return true;
        }
    }
}
