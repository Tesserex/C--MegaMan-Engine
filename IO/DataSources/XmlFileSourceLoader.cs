using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.IO.DataSources
{
    internal class XmlFileSourceLoader : FileSourceLoader
    {
        public override string Extension { get { return "xml"; } }
    }
}
