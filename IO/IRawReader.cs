using MegaMan.Common;

namespace MegaMan.IO
{
    public interface IRawReader : IGameFileReader
    {
        byte[] GetRawData(FilePath path);
    }
}
