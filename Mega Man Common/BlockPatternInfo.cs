using System.Collections.Generic;
using System.Linq;
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

        public BlockPatternInfo Clone()
        {
            return new BlockPatternInfo {
                Entity = Entity,
                Length = Length,
                LeftBoundary = LeftBoundary,
                RightBoundary = RightBoundary,
                Blocks = Blocks.Select(x => x.Clone()).ToList()
            };
        }
    }

    public class BlockInfo
    {
        public PointF pos;
        public int on;
        public int off;

        internal BlockInfo Clone()
        {
            return new BlockInfo {
                pos = pos,
                on = on,
                off = off
            };
        }
    }
}
