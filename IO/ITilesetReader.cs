using MegaMan.Common;

namespace MegaMan.IO
{
    public interface ITilesetReader
    {
        Tileset Load(FilePath path);
    }
}
