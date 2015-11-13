using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Common;

namespace MegaMan.IO
{
    public interface IReaderProvider
    {
        IProjectReader GetProjectReader();
        IStageReader GetStageReader(FilePath path);
        ITilesetReader GetTilesetReader(FilePath path);
    }
}
