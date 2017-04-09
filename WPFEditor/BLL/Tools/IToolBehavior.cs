using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Bll.Tools
{
    public interface IToolBehavior
    {
        bool SuppressContextMenu { get; }

        void Click(ScreenCanvas canvas, Point location);
        void Move(ScreenCanvas canvas, Point location);
        void Release(ScreenCanvas canvas, Point location);
        void RightClick(ScreenCanvas canvas, Point location);
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
