using MegaMan.Common;

namespace MegaMan.IO
{
    public interface IStageReader : IGameFileReader
    {
        StageInfo Load(FilePath path);
    }
}
