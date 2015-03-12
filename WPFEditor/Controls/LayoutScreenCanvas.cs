using System;
using System.Windows;
using System.Windows.Documents;
using MegaMan.Editor.Tools;

namespace MegaMan.Editor.Controls
{
    public class LayoutScreenCanvas : ScreenCanvas
    {
        private ScreenResizeAdorner _adorner;
        private LayoutObjectsLayer _objectsLayer;

        public LayoutScreenCanvas(IToolProvider toolProvider)
            : base(toolProvider)
        {
            _objectsLayer = new LayoutObjectsLayer();
            this.Children.Insert(1, _objectsLayer);

            this.Loaded += AddAdorners;

            _tiles.RenderGrayscale();
        }

        private void AddAdorners(object sender, RoutedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            _adorner = new ScreenResizeAdorner(this, this.Screen);
            adornerLayer.Add(_adorner);
        }

        protected override void ScreenChanged()
        {
            base.ScreenChanged();
            _objectsLayer.Screen = this.Screen;
            if (_adorner != null)
                _adorner.Screen = this.Screen;
        }

        public double RightDistanceTo(ScreenCanvas second)
        {
            if ((this.Margin.Top < second.Margin.Top + second.Screen.PixelHeight) && (this.Margin.Top + this.Screen.PixelHeight > second.Margin.Top))
            {
                return Math.Abs(second.Margin.Left - (this.Margin.Left + this.Screen.PixelWidth));
            }
            else
            {
                return double.PositiveInfinity;
            }
        }

        public double DownDistanceTo(ScreenCanvas second)
        {
            if ((this.Margin.Left < second.Margin.Left + second.Screen.PixelWidth) && (this.Margin.Left + this.Screen.PixelWidth > second.Margin.Left))
            {
                return Math.Abs(second.Margin.Top - (this.Margin.Top + this.Screen.PixelHeight));
            }
            else
            {
                return double.PositiveInfinity;
            }
        }

        public void JoinRightwardTo(ScreenCanvas canvas)
        {
            var tileTopOne = (int)Math.Round(this.Margin.Top / Screen.Tileset.TileSize);
            var tileTopTwo = (int)Math.Round(canvas.Margin.Top / Screen.Tileset.TileSize);

            var startPoint = Math.Max(tileTopOne, tileTopTwo);
            var endPoint = Math.Min(tileTopOne + Screen.Height, tileTopTwo + canvas.Screen.Height);

            var startTileOne = (startPoint - tileTopOne);
            var startTileTwo = (startPoint - tileTopTwo);
            var length = endPoint - startPoint;

            var join = new MegaMan.Common.Join();
            join.screenOne = Screen.Name;
            join.screenTwo = canvas.Screen.Name;
            join.direction = Common.JoinDirection.Both;
            join.type = Common.JoinType.Vertical;
            join.offsetOne = startTileOne;
            join.offsetTwo = startTileTwo;
            join.Size = length;

            Screen.Stage.AddJoin(join);
        }

        public void JoinDownwardTo(ScreenCanvas canvas)
        {
            var tileLeftOne = (int)Math.Round(this.Margin.Left / Screen.Tileset.TileSize);
            var tileLeftTwo = (int)Math.Round(canvas.Margin.Left / Screen.Tileset.TileSize);

            var startPoint = Math.Max(tileLeftOne, tileLeftTwo);
            var endPoint = Math.Min(tileLeftOne + Screen.Width, tileLeftTwo + canvas.Screen.Width);

            var startTileOne = (startPoint - tileLeftOne);
            var startTileTwo = (startPoint - tileLeftTwo);
            var length = endPoint - startPoint;

            var join = new MegaMan.Common.Join();
            join.screenOne = Screen.Name;
            join.screenTwo = canvas.Screen.Name;
            join.direction = Common.JoinDirection.Both;
            join.type = Common.JoinType.Horizontal;
            join.offsetOne = startTileOne;
            join.offsetTwo = startTileTwo;
            join.Size = length;

            Screen.Stage.AddJoin(join);
        }

        protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            _tiles.RenderColor();
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            _tiles.RenderGrayscale();
        }
    }
}
