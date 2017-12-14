using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Editor.Bll.Tools;

namespace MegaMan.Editor.Tools
{
    public class TileBrushCursor : ImageCursor
    {
        private ITileBrush _brush;
        private WriteableBitmap _image;

        public TileBrushCursor(ITileBrush brush)
        {
            _brush = brush;

            _image = new WriteableBitmap((int)Width, (int)Height, 96, 96, PixelFormats.Pbgra32, null);

            var width = _brush.Cells.Length;
            var height = _brush.Cells[0].Length;

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    var cell = _brush.Cells[x][y];
                    var size = cell.tile.Width;
                    var location = cell.tile.Sprite[0].SheetLocation;
                    var rect = new Rect(0, 0, location.Width, location.Height);
                    var source = SpriteBitmapCache.GetOrLoadFrame(cell.tile.Sprite.SheetPath.Absolute, location);

                    _image.Blit(new Rect(x * size, y * size, size, size), source, rect);
                }
            }
        }

        protected override ImageSource CursorImage
        {
            get
            {
                var zoom = Convert.ToDouble(App.Current.Resources["Zoom"] ?? 1);
                return SpriteBitmapCache.Scale(_image, zoom);
            }
        }

        protected override float Width
        {
            get
            {
                return _brush.Width * _brush.Cells[0][0].tile.Width;
            }
        }

        protected override float Height
        {
            get
            {
                return _brush.Height * _brush.Cells[0][0].tile.Height;
            }
        }

        protected override float SnapWidth
        {
            get
            {
                return _brush.Cells[0][0].tile.Width;
            }
        }

        protected override float SnapHeight
        {
            get
            {
                return _brush.Cells[0][0].tile.Height;
            }
        }
    }
}
