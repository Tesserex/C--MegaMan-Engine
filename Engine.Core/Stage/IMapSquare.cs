using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine.Core
{
    public interface IMapSquare
    {
        Rectangle BlockBox { get; }
        TileProperties Properties { get; }
    }
}
