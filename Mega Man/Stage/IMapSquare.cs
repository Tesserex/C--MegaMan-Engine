using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    public interface IMapSquare
    {
        Rectangle BlockBox { get; }
        TileProperties Properties { get; }
    }
}
