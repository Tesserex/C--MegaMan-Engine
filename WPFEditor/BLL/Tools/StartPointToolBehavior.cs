
namespace MegaMan.Editor.Bll.Tools
{
    public class StartPointToolBehavior : IToolBehavior
    {
        public void Click(Controls.ScreenCanvas canvas, Common.Geometry.Point location)
        {
        }

        public void Move(Controls.ScreenCanvas canvas, Common.Geometry.Point location)
        {
        }

        public void Release(Controls.ScreenCanvas canvas, Common.Geometry.Point location)
        {
            canvas.Screen.Stage.SetStartPoint(canvas.Screen, location);
        }

        public void RightClick(Controls.ScreenCanvas canvas, Common.Geometry.Point location)
        {
        }
    }
}
