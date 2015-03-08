
namespace MegaMan.Common
{
    public class TileSprite : Sprite
    {
        private Tileset _tileset;

        public TileSprite(Tileset tileset)
            : base(tileset.TileSize, tileset.TileSize)
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

        public override string SheetPathRelative
        {
            get
            {
                return _tileset.SheetPath.Relative;
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

        public override void Play()
        {
            if (this.Playing)
                return;

            foreach (var t in _tileset)
            {
                t.Sprite.Playing = true;
                t.Sprite.Reset();
            }
        }

        public override void Stop()
        {
            if (!this.Playing)
                return;

            foreach (var t in _tileset)
            {
                t.Sprite.Playing = false;
                t.Sprite.Reset();
            }
        }

        public override void Pause()
        {
            foreach (var t in _tileset)
            {
                t.Sprite.Playing = false;
            }
        }

        public override void Resume()
        {
            foreach (var t in _tileset)
            {
                t.Sprite.Playing = true;
            }
        }
    }
}
