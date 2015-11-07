using System.IO;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO
{
    public interface ITilesetReader
    {
        Tileset Load(Stream stream);
        TileProperties LoadProperties(XElement node);
    }
}
