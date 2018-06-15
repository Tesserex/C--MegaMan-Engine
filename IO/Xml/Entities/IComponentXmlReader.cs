using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Entities
{
    internal interface IComponentXmlReader
    {
        string NodeName { get; }
        IComponentInfo Load(XElement node, Project project, IDataSource dataSource);
    }
}
