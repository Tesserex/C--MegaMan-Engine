using System.IO;
using MegaMan.Common;

namespace MegaMan.IO
{
    public interface IProjectReader
    {
        string Extension { get; }
        Project Load(Stream stream, FilePath path);
    }
}
