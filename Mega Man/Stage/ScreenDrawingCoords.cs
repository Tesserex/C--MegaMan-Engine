using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MegaMan.Engine.Stage
{
    public struct ScreenDrawingCoords
    {
        public int AdjustX;
        public int AdjustY;
        public float OffsetX;
        public float OffsetY;

        public ScreenDrawingCoords(int adjX, int adjY, float offsetX, float offsetY) : this()
        {
            AdjustX = adjX;
            AdjustY = adjY;
            OffsetX = offsetX;
            OffsetY = offsetY;
        }
    }
}
