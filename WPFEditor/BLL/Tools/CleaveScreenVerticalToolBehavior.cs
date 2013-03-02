using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Editor.Bll.Tools
{
    public class CleaveScreenVerticalToolBehavior : IToolBehavior
    {
        public void Click(ScreenDocument screen, Point location) { }

        public void Move(ScreenDocument screen, Point location) { }

        public void Release(ScreenDocument screen, Point location)
        {
            int tilePosX = location.X / screen.Tileset.TileSize;
            screen.CleaveVertically(tilePosX);
        }

        public void RightClick(ScreenDocument screen, Point location) { }
    }
}
