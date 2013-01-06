using System;
using MegaMan.Common.Geometry;

namespace MegaMan.LevelEditor
{
    public interface IToolBehavior
    {
        void Click(ScreenDocument surface, Point location);
        void Move(ScreenDocument surface, Point location);
        void Release(ScreenDocument surface);
        void RightClick(ScreenDocument surface, Point location);
    }

    public class ToolChangedEventArgs : EventArgs
    {
        public ToolChangedEventArgs(IToolBehavior tool)
        {
        }
    }

    public enum ToolType
    {
        Cursor,
        Brush,
        Bucket,
        Join,
        Start,
        Entity,
        Zoom,
        Rectangle,
        Selection
    }
}
