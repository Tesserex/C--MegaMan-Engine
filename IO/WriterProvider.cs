using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.IO.Xml;

namespace MegaMan.IO
{
    public class WriterProvider : IWriterProvider
    {
        public IProjectWriter GetProjectWriter()
        {
            return new ProjectXmlWriter();
        }

        public IStageWriter GetStageWriter()
        {
            return new StageXmlWriter();
        }

        public ITilesetWriter GetTilesetWriter()
        {
            return new TilesetXmlWriter();
        }
    }
}
