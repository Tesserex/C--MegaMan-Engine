using MegaMan.Common;

namespace MegaMan.IO
{
    public interface IReaderProvider
    {
        IProjectReader GetProjectReader();
        IStageReader GetStageReader(FilePath path);
        ITilesetReader GetTilesetReader(FilePath path);
        IRawReader GetRawReader();
    }
}
