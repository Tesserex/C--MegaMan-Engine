using MegaMan.Common;
using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Engine
{
    public interface IMapSquare
    {
        RectangleF BlockBox { get; }
        TileProperties Properties { get; }
    }
}
