using System;
using System.Windows.Media;
using MegaMan.Common;

namespace MegaMan.Editor.Tools
{
    public class SpriteCursor : ImageCursor
    {
        private Sprite _sprite;

        public SpriteCursor(Sprite sprite) : base(sprite.HotSpot)
        {
            _sprite = sprite;
            DrawOutline = false;
        }

        protected override ImageSource CursorImage
        {
            get
            {
                var zoom = Convert.ToDouble(App.Current.Resources["Zoom"] ?? 1);
                var image = SpriteBitmapCache.GetOrLoadFrame(_sprite.SheetPath.Absolute, _sprite.CurrentFrame.SheetLocation);
                return SpriteBitmapCache.Scale(image, zoom);
            }
        }

        protected override float Width { get { return _sprite.Width; } }
        protected override float Height { get { return _sprite.Height; } }
        protected override float SnapWidth { get { return 1; } }
        protected override float SnapHeight { get { return 1; } }
    }
}
