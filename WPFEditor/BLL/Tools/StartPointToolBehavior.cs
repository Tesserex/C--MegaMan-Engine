
using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Bll.Tools
{
    public class StartPointToolBehavior : IToolBehavior
    {
        private readonly int _snapX;
        private readonly int _snapY;

        public StartPointToolBehavior(int snapX, int snapY)
        {
            _snapX = snapX;
            _snapY = snapY;
        }

        public void Click(ScreenCanvas canvas, Point location)
        {
        }

        public void Move(ScreenCanvas canvas, Point location)
        {
        }

        public void Release(ScreenCanvas canvas, Point location)
        {
            var snappedPoint = new Point(
                (location.X / _snapX) * _snapX,
                (location.Y / _snapY) * _snapY);

            canvas.Screen.Stage.SetStartPoint(canvas.Screen, snappedPoint);
        }

        public void RightClick(ScreenCanvas canvas, Point location)
        {
        }

        public bool SuppressContextMenu { get { return false; } }
    }
}
