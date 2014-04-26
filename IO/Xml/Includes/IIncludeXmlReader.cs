using MegaMan.Common;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    internal interface IIncludeXmlReader
    {
        void Load(Project project, XElement xmlNode);
    }
}
