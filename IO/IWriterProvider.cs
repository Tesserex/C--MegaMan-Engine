using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.IO
{
    public interface IWriterProvider
    {
        IProjectWriter GetProjectWriter();
        IStageWriter GetStageWriter();
        ITilesetWriter GetTilesetWriter();
    }
}
