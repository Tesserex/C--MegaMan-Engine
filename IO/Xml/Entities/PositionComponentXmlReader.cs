using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Entities
{
    internal class PositionComponentXmlReader : IComponentXmlReader
    {
        public string NodeName
        {
            get { return "Position"; }
        }

        public IComponentInfo Load(XElement node, Project project)
        {
            var posInfo = new PositionComponentInfo();
            posInfo.PersistOffscreen = node.TryAttribute<bool>("persistoffscreen");
            return posInfo;
        }
    }
}
