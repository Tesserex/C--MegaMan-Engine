using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.IO.Xml;

namespace MegaMan.IO
{
    public class ReaderProvider : IReaderProvider
    {
        public IProjectReader GetProjectReader()
        {
            return new ProjectXmlReader();
        }
    }
}
