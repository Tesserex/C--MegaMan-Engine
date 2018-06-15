using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Bll.Tools
{
    public class TestLocationToolBehavior : IToolBehavior
    {
        public void Click(ScreenCanvas canvas, Point location)
        {
        }

        public void Move(ScreenCanvas canvas, Point location)
        {
        }

        public void Release(ScreenCanvas canvas, Point location)
        {
            var args = new TestLocationSelectedEventArgs {
                Screen = canvas.Screen.Name,
                X = location.X,
                Y = location.Y
            };

            ViewModelMediator.Current.GetEvent<TestLocationSelectedEventArgs>().Raise(this, args);
        }

        public void RightClick(ScreenCanvas canvas, Point location)
        {
        }

        public bool SuppressContextMenu { get { return false; } }
    }
}
