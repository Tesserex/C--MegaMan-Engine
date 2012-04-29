using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MegaMan.LevelEditor
{
    public class Zoom : ITool
    {
        public Image Icon
        {
            get { return Properties.Resources.ZoomHS; }
        }

        public bool IconSnap
        {
            get { return false; }
        }

        public bool IsIconCursor
        {
            get { return true; }
        }

        public void Click(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
            var zoomPoint = new Point(surface.Location.X + location.X, surface.Location.Y + location.Y);
            var form = (StageForm)(surface.Parent);
            form.ZoomIn(zoomPoint);
        }

        public void Move(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
        }

        public void Release(ScreenDrawingSurface surface)
        {
        }

        public void RightClick(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
            var zoomPoint = new Point(surface.Location.X + location.X, surface.Location.Y + location.Y);
            var form = (StageForm)(surface.Parent);
            form.ZoomOut(zoomPoint);
        }

        public System.Drawing.Point IconOffset
        {
            get { return Point.Empty; }
        }
    }
}
