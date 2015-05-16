using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO
{
    public interface ITilesetReader
    {
        Tileset Load(FilePath path);
        TileProperties LoadProperties(XElement node);
    }
}
