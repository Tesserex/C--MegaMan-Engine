using System;
using System.Drawing;

namespace MegaMan.LevelEditor
{
    public interface ITool
    {
        Image Icon { get; }
        bool IconSnap { get; }
        bool IsIconCursor { get; }
        void Click(ScreenDrawingSurface surface, Point location);
        void Move(ScreenDrawingSurface surface, Point location);
        void Release(ScreenDrawingSurface surface);
        void RightClick(ScreenDrawingSurface surface, Point location);
        Point IconOffset { get; }
    }

    public class ToolChangedEventArgs : EventArgs
    {
        public ToolChangedEventArgs(ITool tool)
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
