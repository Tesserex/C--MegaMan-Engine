using MegaMan.Common;

namespace MegaMan.Engine.Core.Entities
{
    public interface ITilePropertiesSource
    {
        TileProperties GetProperties(string name);
    }
}
