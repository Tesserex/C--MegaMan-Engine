
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

        public void Click(Controls.ScreenCanvas canvas, Common.Geometry.Point location)
        {
        }

        public void Move(Controls.ScreenCanvas canvas, Common.Geometry.Point location)
        {
        }

        public void Release(Controls.ScreenCanvas canvas, Common.Geometry.Point location)
        {
            var snappedPoint = new Common.Geometry.Point(
                (location.X / _snapX) * _snapX,
                (location.Y / _snapY) * _snapY);

            canvas.Screen.Stage.SetStartPoint(canvas.Screen, snappedPoint);
        }

        public void RightClick(Controls.ScreenCanvas canvas, Common.Geometry.Point location)
        {
        }
    }
}
