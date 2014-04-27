using MegaMan.Common;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    internal interface IIncludeXmlReader
    {
        string NodeName { get; }
        void Load(Project project, XElement xmlNode);
    }
}
