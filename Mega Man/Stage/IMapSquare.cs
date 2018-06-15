using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    public interface IMapSquare
    {
        RectangleF BlockBox { get; }
        TileProperties Properties { get; }
    }
}
