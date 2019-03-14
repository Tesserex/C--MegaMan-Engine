using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Bll.Tools
{
    public class CleaveScreenVerticalToolBehavior : IToolBehavior
    {
        public void Click(ScreenCanvas canvas, Point location) { }

        public void Move(ScreenCanvas canvas, Point location) { }

        public void Release(ScreenCanvas canvas, Point location)
        {
            int tilePosX = location.X / canvas.Screen.Tileset.TileSize;
            var action = new SplitScreenAction(canvas.Screen, tilePosX);
            action.Execute();
            canvas.Screen.Stage.PushHistoryAction(action);
        }

        public void RightClick(ScreenCanvas canvas, Point location) { }

        public bool SuppressContextMenu { get { return false; } }
    }
}
