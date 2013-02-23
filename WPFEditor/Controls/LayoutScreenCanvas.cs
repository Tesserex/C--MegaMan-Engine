using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace MegaMan.Editor.Controls
{
    public class LayoutScreenCanvas : ScreenCanvas
    {
        private bool _dragging;
        private Vector _dragAnchorOffset;

        public event EventHandler ScreenDropped;

        public LayoutScreenCanvas() : base()
        {
            this.Loaded += AddAdorners;
        }

        private void AddAdorners(object sender, RoutedEventArgs e)
        {
            var adornerLayer = AdornerLayer.GetAdornerLayer(this);

            adornerLayer.Add(new ScreenResizeAdorner(this, this.Screen));
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
            var tileTopOne = (int)(this.Margin.Top / Screen.Tileset.TileSize);
            var tileTopTwo = (int)(canvas.Margin.Top / Screen.Tileset.TileSize);

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
            var tileLeftOne = (int)(this.Margin.Left / Screen.Tileset.TileSize);
            var tileLeftTwo = (int)(canvas.Margin.Left / Screen.Tileset.TileSize);

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

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            BeginLayoutDrag(e.GetPosition(this));
        }

        protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            EndLayoutDrag();
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (_dragging)
            {
                var mousePosition = e.GetPosition((IInputElement)this.Parent);  

                this.Margin = new Thickness(mousePosition.X - _dragAnchorOffset.X, mousePosition.Y - _dragAnchorOffset.Y, 0, 0);
            }
        }

        private void BeginLayoutDrag(Point anchor)
        {
            _dragging = true;
            _dragAnchorOffset = new Vector(anchor.X, anchor.Y);

            CaptureMouse();

            Canvas.SetZIndex(this, 100);
        }

        private void EndLayoutDrag()
        {
            _dragging = false;

            ReleaseMouseCapture();

            Canvas.SetZIndex(this, 1);

            if (ScreenDropped != null)
            {
                ScreenDropped(this, EventArgs.Empty);
            }
        }
    }
}
