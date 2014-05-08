using MegaMan.Common;

namespace MegaMan.Editor.Bll.Factories
{
    public interface ITilesetDocumentFactory
    {
        TilesetDocument CreateNew(string directory);
        TilesetDocument Load(FilePath filePath);
    }
}
