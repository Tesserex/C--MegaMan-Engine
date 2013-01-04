using System;
using MegaMan.Common.Geometry;

namespace MegaMan.LevelEditor
{
    public interface IScreenSurface
    {
        ScreenDocument Screen { get; }
    }

    public interface IToolBehavior
    {
        void Click(IScreenSurface surface, Point location);
        void Move(IScreenSurface surface, Point location);
        void Release(IScreenSurface surface);
        void RightClick(IScreenSurface surface, Point location);
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
