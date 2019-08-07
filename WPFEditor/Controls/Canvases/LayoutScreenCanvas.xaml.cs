using System;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using MegaMan.Common;
using MegaMan.Editor.Controls.Adorners;
using MegaMan.Editor.Tools;

namespace MegaMan.Editor.Controls
{
    public partial class LayoutScreenCanvas : ScreenCanvas
    {
        private ScreenResizeAdorner _adorner;
        private LayoutObjectsLayer _objectsLayer;

        public LayoutScreenCanvas(IToolProvider toolProvider)
            : base(toolProvider)
        {
            InitializeComponent();

            _objectsLayer = new LayoutObjectsLayer();
            Children.Insert(1, _objectsLayer);

            Loaded += AddAdorners;

            _tiles.RenderGrayscale();
        }

        private void AddAdorners(object sender, RoutedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);
            _adorner = new ScreenResizeAdorner(this, Screen);
            adornerLayer.Add(_adorner);
        }

        protected override void ScreenChanged()
        {
            base.ScreenChanged();
            _objectsLayer.Screen = Screen;
            if (_adorner != null)
                _adorner.Screen = Screen;
        }

        public double RightDistanceTo(ScreenCanvas second)
        {
            if ((Margin.Top < second.Margin.Top + second.Screen.PixelHeight) && (Margin.Top + Screen.PixelHeight > second.Margin.Top))
            {
                return Math.Abs(second.Margin.Left - (Margin.Left + Screen.PixelWidth));
            }

            return double.PositiveInfinity;
        }

        public double DownDistanceTo(ScreenCanvas second)
        {
            if ((Margin.Left < second.Margin.Left + second.Screen.PixelWidth) && (Margin.Left + Screen.PixelWidth > second.Margin.Left))
            {
                return Math.Abs(second.Margin.Top - (Margin.Top + Screen.PixelHeight));
            }

            return double.PositiveInfinity;
        }

        public void JoinRightwardTo(ScreenCanvas canvas)
        {
            var tileTopOne = (int)Math.Round(Margin.Top / Screen.Tileset.TileSize);
            var tileTopTwo = (int)Math.Round(canvas.Margin.Top / Screen.Tileset.TileSize);

            var startPoint = Math.Max(tileTopOne, tileTopTwo);
            var endPoint = Math.Min(tileTopOne + Screen.Height, tileTopTwo + canvas.Screen.Height);

            var startTileOne = (startPoint - tileTopOne);
            var startTileTwo = (startPoint - tileTopTwo);
            var length = endPoint - startPoint;

            var join = new Join();
            join.ScreenOne = Screen.Name;
            join.ScreenTwo = canvas.Screen.Name;
            join.Direction = JoinDirection.Both;
            join.Type = JoinType.Vertical;
            join.OffsetOne = startTileOne;
            join.OffsetTwo = startTileTwo;
            join.Size = length;

            Screen.Stage.AddJoin(join);
        }

        public void JoinDownwardTo(ScreenCanvas canvas)
        {
            var tileLeftOne = (int)Math.Round(Margin.Left / Screen.Tileset.TileSize);
            var tileLeftTwo = (int)Math.Round(canvas.Margin.Left / Screen.Tileset.TileSize);

            var startPoint = Math.Max(tileLeftOne, tileLeftTwo);
            var endPoint = Math.Min(tileLeftOne + Screen.Width, tileLeftTwo + canvas.Screen.Width);

            var startTileOne = (startPoint - tileLeftOne);
            var startTileTwo = (startPoint - tileLeftTwo);
            var length = endPoint - startPoint;

            var join = new Join();
            join.ScreenOne = Screen.Name;
            join.ScreenTwo = canvas.Screen.Name;
            join.Direction = JoinDirection.Both;
            join.Type = JoinType.Horizontal;
            join.OffsetOne = startTileOne;
            join.OffsetTwo = startTileTwo;
            join.Size = length;

            Screen.Stage.AddJoin(join);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            _tiles.RenderColor();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            _tiles.RenderGrayscale();
        }

        private void CloneClicked(object sender, RoutedEventArgs e)
        {
            Screen.Clone();
        }

        private void DeleteClicked(object sender, RoutedEventArgs e)
        {
            Screen.Delete();
        }
    }
}
