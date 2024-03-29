﻿using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls
{
    public class TileScreenLayer : ScreenLayer
    {
        private bool _grayscale;
        private WriteableBitmap _colorBitmap;
        private WriteableBitmap _grayBitmap;
        private static WriteableBitmap _blankTileImage;

        static TileScreenLayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TileScreenLayer), new FrameworkPropertyMetadata(typeof(TileScreenLayer)));
            _blankTileImage = BitmapFactory.ConvertToPbgra32Format(SpriteBitmapCache.GetResource("blank_tile.png"));
        }

        public void RenderColor()
        {
            _grayscale = false;
            InvalidateVisual();
        }

        public void RenderGrayscale()
        {
            _grayscale = true;
            InvalidateVisual();
        }

        protected override void UnbindScreen(ScreenDocument oldScreen)
        {
            oldScreen.TileChanged -= Update;
            oldScreen.Resized -= (x,y) => Update();
        }

        protected override void BindScreen(ScreenDocument newScreen)
        {
            newScreen.TileChanged += Update;
            newScreen.Resized += (x, y) => Update();
        }

        protected override void Update()
        {
            RenderScreen();
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_grayBitmap == null || _colorBitmap == null)
                RenderScreen();

            if (_grayscale)
                dc.DrawImage(_grayBitmap, new Rect(0, 0, _grayBitmap.PixelWidth, _grayBitmap.PixelHeight));
            else
                dc.DrawImage(_colorBitmap, new Rect(0, 0, _colorBitmap.PixelWidth, _colorBitmap.PixelHeight));
        }

        protected override void OnZoomChanged()
        {
            base.OnZoomChanged();
            RenderScreen();
        }

        private void RenderScreen()
        {
            var bitmap = new WriteableBitmap((int)(Screen.PixelWidth * Zoom), (int)(Screen.PixelHeight * Zoom), 96, 96, PixelFormats.Pbgra32, null);
            var tilesetDocument = Screen.Stage.Tileset;
            var size = tilesetDocument.Tileset.TileSize * Zoom;

            for (int y = 0; y < Screen.Height; y++)
            {
                for (int x = 0; x < Screen.Width; x++)
                {
                    var tile = Screen.TileAt(x, y);
                    if (tile != null && tile.Id >= 0)
                    {
                        var location = tile.Sprite[0].SheetLocation;
                        var rect = new Rect(0, 0, location.Width, location.Height);
                        var image = SpriteBitmapCache.GetOrLoadFrame(Screen.Tileset.SheetPath.Absolute, location);
                        bitmap.Blit(new Rect(x * size, y * size, size, size), image, rect);
                    }
                    else
                    {
                        bitmap.Blit(new Rect(x * size, y * size, size, size), _blankTileImage, new Rect(0, 0, 16, 16));
                    }
                }
            }

            _colorBitmap = bitmap;
            _colorBitmap.Freeze();

            var grayscale = new FormatConvertedBitmap(bitmap, PixelFormats.Gray16, BitmapPalettes.Gray256, 1);
            var bmp = BitmapFactory.ConvertToPbgra32Format(grayscale);
            bmp.Freeze();
            _grayBitmap = bmp;
        }
    }
}
