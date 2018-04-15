using System;
using System.Windows.Media;
using MegaMan.Common;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Tools
{
    public class SpriteCursor : ImageCursor
    {
        private SpriteModel _sprite;
        private int _snapWidth;
        private int _snapHeight;

        public SpriteCursor(SpriteModel sprite, int snapWidth = 1, int snapHeight = 1) : base(sprite.HotSpot)
        {
            _sprite = sprite;
            _snapWidth = snapWidth;
            _snapHeight = snapHeight;
            DrawOutline = false;
        }

        protected override ImageSource CursorImage
        {
            get
            {
                var zoom = Convert.ToDouble(App.Current.Resources["Zoom"] ?? 1);
                return _sprite.GetImageSource(zoom);
            }
        }

        protected override float Width { get { return _sprite.Width; } }
        protected override float Height { get { return _sprite.Height; } }
        protected override float SnapWidth { get { return _snapWidth; } }
        protected override float SnapHeight { get { return _snapHeight; } }
    }
}
