using MegaMan.Common;

namespace MegaMan.IO
{
    public interface IProjectReader
    {
        Project Load(string filePath);
    }
}
