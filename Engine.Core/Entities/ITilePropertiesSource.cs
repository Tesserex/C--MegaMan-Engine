using MegaMan.Common;

namespace MegaMan.Engine.Entities
{
    public interface ITilePropertiesSource
    {
        TileProperties GetProperties(string name);
    }
}
