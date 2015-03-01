using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Editor.Bll.Tools;

namespace MegaMan.Editor.Tools
{
    public class MultiTileCursor : SingleTileCursor
    {
        private MultiTileBrush _brush;

        public MultiTileCursor(MultiTileBrush brush)
        {
            _brush = brush;

            var image = new WriteableBitmap((int)Width, (int)Height, 96, 96, PixelFormats.Pbgra32, null);
        }

        protected override ImageSource CursorImage
        {
            get
            {
                var cursor = new WriteableBitmap((int)Width, (int)Height, 96, 96, PixelFormats.Pbgra32, null);

                var width = _brush.Cells.Length;
                var height = _brush.Cells[0].Length;

                for (var x = 0; x < width; x++)
                {
                    for (var y = 0; y < height; y++)
                    {
                        var cell = _brush.Cells[x][y];
                        var size = cell.tile.Width;
                        var location = cell.tile.Sprite.CurrentFrame.SheetLocation;
                        var rect = new Rect(0, 0, location.Width, location.Height);
                        var source = SpriteBitmapCache.GetOrLoadFrame(cell.tile.Sprite.SheetPath.Absolute, location);

                        cursor.Blit(new Rect(x * size, y * size, size, size), source, rect);
                    }
                }

                return cursor;
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
