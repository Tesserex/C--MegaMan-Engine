using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Common.Geometry;

namespace MegaMan.Editor.Controls
{
    public class EntityMoveData
    {
        public EntityPlacementControl Control { get; set; }
        public Point StartPoint { get; set; }
        public int SnapX { get; set; }
        public int SnapY { get; set; }
    }
}
