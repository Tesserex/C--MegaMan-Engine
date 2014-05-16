using MegaMan.Common;

namespace MegaMan.Editor.Bll.Factories
{
    public interface ITilesetDocumentFactory
    {
        TilesetDocument CreateNew(FilePath filePath);
        TilesetDocument Load(FilePath filePath);
    }
}
