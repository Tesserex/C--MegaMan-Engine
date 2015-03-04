using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public interface IGameObjectXmlReader
    {
        void Load(Project project, XElement node);
    }
}
