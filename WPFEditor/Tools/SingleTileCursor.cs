using System;
using System.Windows.Media;
using MegaMan.Common;

namespace MegaMan.Editor.Tools
{
    public class SingleTileCursor : ImageCursor
    {
        private Tile _tile;

        public SingleTileCursor(Tile tile)
        {
            _tile = tile;
        }

        protected override ImageSource CursorImage
        {
            get
            {
                var zoom = Convert.ToDouble(App.Current.Resources["Zoom"] ?? 1);
                var image = SpriteBitmapCache.GetOrLoadFrame(_tile.Sprite.SheetPath.Absolute, _tile.Sprite.CurrentFrame.SheetLocation);
                return SpriteBitmapCache.Scale(image, zoom);
            }
        }

        protected override float Width { get { return _tile.Width; } }
        protected override float Height { get { return _tile.Height; } }
        protected override float SnapWidth { get { return _tile.Width; } }
        protected override float SnapHeight { get { return _tile.Height; } }
    }
}
