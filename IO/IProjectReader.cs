using System.IO;
using MegaMan.Common;

namespace MegaMan.IO
{
    public interface IProjectReader : IGameFileReader
    {
        string Extension { get; }
        Project Load();
    }
}
