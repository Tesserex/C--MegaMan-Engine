using System.Collections.Generic;
using System.IO;
using MegaMan.Common;

namespace MegaMan.IO.DataSources
{
    public interface IDataSource
    {
        string Extension { get; }
        void Init(string path);
        Stream GetData(FilePath path);
        FilePath GetGameFile();
        IEnumerable<FilePath> GetFilesInFolder(FilePath folderPath);
    }
}
