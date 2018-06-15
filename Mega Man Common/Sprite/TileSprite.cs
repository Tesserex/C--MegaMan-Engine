
namespace MegaMan.Common
{
    public class TileSprite : Sprite
    {
        private Tileset tileset;

        public TileSprite(Tileset tileset)
            : base(tileset.TileSize, tileset.TileSize)
        {
            this.tileset = tileset;
        }

        public TileSprite(Tileset tileset, Sprite copy)
            : base(copy)
        {
            this.tileset = tileset;
        }

        public override FilePath SheetPath
        {
            get
            {
                return tileset.SheetPath;
            }
            set
            {
                tileset.SheetPath = value;
            }
        }

        public override string SheetPathRelative
        {
            get
            {
                return tileset.SheetPath.Relative;
            }
        }

        public override int Width
        {
            get
            {
                return tileset.TileSize;
            }
        }

        public override int Height
        {
            get
            {
                return tileset.TileSize;
            }
        }
    }
}
