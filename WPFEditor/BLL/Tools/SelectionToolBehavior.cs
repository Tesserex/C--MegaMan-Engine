using System;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Bll.Tools
{
    public class SelectionToolBehavior : IToolBehavior
    {
        private int tx1, ty1, tx2, ty2;
        private bool held;

        public void Click(ScreenCanvas surface, Point location)
        {
            tx1 = location.X / surface.Screen.Tileset.TileSize;
            ty1 = location.Y / surface.Screen.Tileset.TileSize;
            tx2 = tx1;
            ty2 = ty1;
            held = true;
        }

        public void Move(ScreenCanvas surface, Point location)
        {
            if (held)
            {
                tx2 = (int)Math.Round(location.X / (float)surface.Screen.Tileset.TileSize);
                ty2 = (int)Math.Round(location.Y / (float)surface.Screen.Tileset.TileSize);

                SetSelection(surface);
            }
        }

        public void Release(ScreenCanvas surface, Point location)
        {
            held = false;
        }

        private void SetSelection(ScreenCanvas surface)
        {
            surface.Screen.SetSelection(
                Math.Min(tx1, tx2),
                Math.Min(ty1, ty2),
                Math.Abs(tx2 - tx1),
                Math.Abs(ty2 - ty1)
            );
        }

        public void RightClick(ScreenCanvas surface, Point location)
        {
            surface.Screen.SetSelection(0, 0, 0, 0);
        }
    }
}
