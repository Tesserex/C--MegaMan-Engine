using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MegaMan.LevelEditor
{
    public class SelectionTool : ITool
    {
        private int tx1, ty1, tx2, ty2;
        private bool held;

        private ScreenDrawingSurface currentSurface;

        public System.Drawing.Image Icon
        {
            get { return Properties.Resources.cross; }
        }

        public bool IconSnap
        {
            get { return true; }
        }

        public bool IsIconCursor
        {
            get { return false; }
        }

        public void Click(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
            if (currentSurface != null)
            {
                var g = surface.GetToolLayerGraphics();
                if (g != null)
                {
                    g.Clear(Color.Transparent);
                }
            }

            currentSurface = surface;

            tx1 = location.X / surface.Screen.Tileset.TileSize;
            ty1 = location.Y / surface.Screen.Tileset.TileSize;
            tx2 = tx1;
            ty2 = ty1;
            held = true;
        }

        public void Move(ScreenDrawingSurface surface, System.Drawing.Point location)
        {
            if (held)
            {
                tx2 = location.X / surface.Screen.Tileset.TileSize;
                ty2 = location.Y / surface.Screen.Tileset.TileSize;

                Release(surface);
            }
        }

        public void Release(ScreenDrawingSurface surface)
        {
            surface.SetSelection(
                Math.Min(tx1, tx2),
                Math.Min(ty1, ty2),
                Math.Abs(tx2 - tx1),
                Math.Abs(ty2 - ty1)
            );
        }

        public void RightClick(ScreenDrawingSurface surface, System.Drawing.Point location)
        {

        }

        public System.Drawing.Point IconOffset
        {
            get { return new Point(-7, -7); }
        }
    }
}
