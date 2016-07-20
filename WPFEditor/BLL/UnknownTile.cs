using MegaMan.Common;

namespace MegaMan.Editor.Bll
{
    internal class UnknownTile : Tile
    {
        public UnknownTile(Tileset tileset) : base(-1, new TileSprite(tileset)) { }
    }
}
