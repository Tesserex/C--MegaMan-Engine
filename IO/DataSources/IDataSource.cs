using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.IO.DataSources
{
    public interface IDataSource
    {
        string Extension { get; }
        void Init(string path);
        Stream GetData(FilePath path);
        FilePath GetGameFile();
    }
}
