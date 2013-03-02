using System;
using MegaMan.Common.Geometry;

namespace MegaMan.Editor.Bll.Tools
{
    public interface IToolBehavior
    {
        void Click(ScreenDocument screen, Point location);
        void Move(ScreenDocument screen, Point location);
        void Release(ScreenDocument screen, Point location);
        void RightClick(ScreenDocument screen, Point location);
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
