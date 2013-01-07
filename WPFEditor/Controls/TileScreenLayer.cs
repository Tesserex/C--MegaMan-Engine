using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls
{
    public class TileScreenLayer : ScreenLayer
    {
        static TileScreenLayer()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TileScreenLayer), new FrameworkPropertyMetadata(typeof(TileScreenLayer)));
        }

        protected override void UnbindScreen(ScreenDocument oldScreen)
        {
            oldScreen.TileChanged -= Update;
        }

        protected override void BindScreen(ScreenDocument newScreen)
        {
            newScreen.TileChanged += Update;
            _surface = new RenderTargetBitmap(newScreen.PixelWidth, newScreen.PixelHeight, 96, 96, PixelFormats.Default);
        }

        protected override void Update()
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var size = Screen.Tileset.TileSize;

            for (int y = 0; y < Screen.Height; y++)
            {
                for (int x = 0; x < Screen.Width; x++)
                {
                    var tile = Screen.TileAt(x, y);
                    var location = tile.Sprite.CurrentFrame.SheetLocation;

                    var image = SpriteBitmapCache.GetOrLoadFrame(Screen.Tileset.SheetPath.Absolute, location);

                    dc.DrawImage(image, new Rect(x * size, y * size, size, size));
                }
            }
        }
    }
}
