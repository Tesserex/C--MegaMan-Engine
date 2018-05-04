using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Entities
{
    internal class InputComponentXmlReader : IComponentXmlReader
    {
        public string NodeName
        {
            get { return "Input"; }
        }

        public IComponentInfo Load(XElement node, Project project, IDataSource dataSource)
        {
            return new InputComponentInfo();
        }
    }
}
