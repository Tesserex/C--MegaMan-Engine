using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Bll.Tools
{
    public class ContinuePointToolBehavior : IToolBehavior
    {
        public void Click(ScreenCanvas canvas, Point location)
        {
        }

        public void Move(ScreenCanvas canvas, Point location)
        {
        }

        public void Release(ScreenCanvas canvas, Point location)
        {
            canvas.Screen.Stage.AddContinuePoint(canvas.Screen, location);
        }

        public void RightClick(ScreenCanvas canvas, Point location)
        {
        }

        public bool SuppressContextMenu { get { return false; } }
    }
}
