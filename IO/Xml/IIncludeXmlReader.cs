using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO.Xml
{
    internal interface IIncludeXmlReader
    {
        string NodeName { get; }
        IIncludedObject Load(Project project, XElement xmlNode);
    }
}
