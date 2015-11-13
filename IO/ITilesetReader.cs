using System.IO;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO
{
    public interface ITilesetReader : IGameFileReader
    {
        Tileset Load(FilePath path);
        TileProperties LoadProperties(XElement node);
    }
}
