using System.Collections.Generic;
using MegaMan.Common.Geometry;

namespace MegaMan.Common
{
    public class BlockPatternInfo
    {
        public string Entity { get; set; }
        public int Length { get; set; }
        public List<BlockInfo> Blocks { get; set; }
        public int LeftBoundary { get; set; }
        public int RightBoundary { get; set; }
    }

    public class BlockInfo
    {
        public PointF pos;
        public int on;
        public int off;
    }
}
