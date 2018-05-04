using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Entities
{
    internal class PositionComponentXmlReader : IComponentXmlReader
    {
        public string NodeName
        {
            get { return "Position"; }
        }

        public IComponentInfo Load(XElement node, Project project, IDataSource dataSource)
        {
            var posInfo = new PositionComponentInfo();
            posInfo.PersistOffscreen = node.TryAttribute<bool>("persistoffscreen");
            return posInfo;
        }
    }
}
