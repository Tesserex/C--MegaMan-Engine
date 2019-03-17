using MegaMan.Common;

namespace MegaMan.Editor.Bll
{
    internal class UnknownTile : Tile
    {
        public const int UnknownId = -1;

        public UnknownTile(Tileset tileset) : base(UnknownId, new TileSprite(tileset)) { }
    }
}
