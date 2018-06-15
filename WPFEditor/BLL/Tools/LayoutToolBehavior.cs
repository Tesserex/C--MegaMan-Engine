using System.Windows;
using System.Windows.Controls;
using MegaMan.Editor.Controls;
using MegaMan.Editor.Mediator;
using Point = MegaMan.Common.Geometry.Point;

namespace MegaMan.Editor.Bll.Tools
{
    public class LayoutToolBehavior : IToolBehavior
    {
        private bool _dragging;
        private Vector _dragAnchorOffset;

        public bool SuppressContextMenu { get { return false; } }

        public void Click(ScreenCanvas canvas, Point location)
        {
            _dragging = true;
            _dragAnchorOffset = new Vector(location.X, location.Y);

            canvas.CaptureMouse();

            Canvas.SetZIndex(canvas, 100);
        }

        public void Move(ScreenCanvas canvas, Point location)
        {
            if (_dragging)
            {
                canvas.Margin = new Thickness(canvas.Margin.Left + location.X - _dragAnchorOffset.X, canvas.Margin.Top + location.Y - _dragAnchorOffset.Y, 0, 0);
            }
        }

        public void Release(ScreenCanvas canvas, Point location)
        {
            _dragging = false;

            canvas.ReleaseMouseCapture();

            Canvas.SetZIndex(canvas, 1);

            ViewModelMediator.Current.GetEvent<LayoutScreenDroppedEventArgs>().Raise(canvas, new LayoutScreenDroppedEventArgs { Canvas = canvas });
        }

        public void RightClick(ScreenCanvas canvas, Point location)
        {
        }
    }
}
