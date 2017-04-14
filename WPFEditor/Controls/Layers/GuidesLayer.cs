using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls
{
    public class GuidesLayer : ScreenLayer
    {
        private static readonly Pen InnerPen = new Pen(Brushes.LightGray, 1);
        private static readonly Pen OuterPen = new Pen(Brushes.DarkSlateGray, 1);

        private WriteableBitmap _propertiesBitmap;

        private bool _borderVisible;
        private bool _propertiesVisible;

        public GuidesLayer()
            : base()
        {
            ViewModelMediator.Current.GetEvent<LayerVisibilityChangedEventArgs>().Subscribe(LayerVisibilityChanged, true);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
        }

        private void LayerVisibilityChanged(object sender, LayerVisibilityChangedEventArgs e)
        {
            _borderVisible = e.BordersVisible;
            _propertiesVisible = e.TilePropertiesVisible;
        }

        protected override void UnbindScreen(ScreenDocument oldScreen)
        {
            oldScreen.TileChanged -= DrawProperties;
        }

        protected override void BindScreen(ScreenDocument newScreen)
        {
            newScreen.TileChanged += DrawProperties;
        }

        protected override void Update()
        {
            DrawProperties();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            if (_borderVisible)
            {
                dc.DrawRectangle(null, OuterPen, new Rect(0.5, 0.5, Zoom * Screen.PixelWidth - 1, Zoom * Screen.PixelHeight - 1));
                dc.DrawRectangle(null, InnerPen, new Rect(1.5, 1.5, Zoom * Screen.PixelWidth - 3, Zoom * Screen.PixelHeight - 3));
            }

            if (_propertiesVisible)
            {
                dc.PushOpacity(0.5);
                dc.DrawImage(_propertiesBitmap, new Rect(0, 0, Zoom * Screen.PixelWidth, Zoom * Screen.PixelHeight));
                dc.Pop();
            }
        }

        private void DrawProperties()
        {
            _propertiesBitmap = new WriteableBitmap(Screen.PixelWidth, Screen.PixelHeight, 96, 96, PixelFormats.Pbgra32, null);
            RenderOptions.SetBitmapScalingMode(_propertiesBitmap, BitmapScalingMode.NearestNeighbor);

            var size = Screen.Tileset.TileSize;

            for (int y = 0; y < Screen.Height; y++)
            {
                for (int x = 0; x < Screen.Width; x++)
                {
                    var tile = Screen.TileAt(x, y);

                    if (tile.Properties.Sinking != 0 || tile.Properties.PushX != 0 || tile.Properties.PushY != 0)
                    {
                        _propertiesBitmap.FillRectangle(x * size, y * size, (x + 1) * size, (y + 1) * size, Colors.Purple);
                    }
                    else if (tile.Properties.Lethal)
                    {
                        _propertiesBitmap.FillRectangle(x * size, y * size, (x + 1) * size, (y + 1) * size, Colors.Red);
                    }
                    else if (tile.Properties.Blocking)
                    {
                        _propertiesBitmap.FillRectangle(x * size, y * size, (x + 1) * size, (y + 1) * size, Colors.Green);
                    }
                    else if (tile.Properties.Climbable)
                    {
                        _propertiesBitmap.FillRectangle(x * size, y * size, (x + 1) * size, (y + 1) * size, Colors.Yellow);
                    }
                    else if (tile.Properties.GravityMult < 1)
                    {
                        _propertiesBitmap.FillRectangle(x * size, y * size, (x + 1) * size, (y + 1) * size, Colors.LightBlue);
                    }
                }
            }
        }
    }
}
