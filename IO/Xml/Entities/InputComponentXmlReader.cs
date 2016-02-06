using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Entities
{
    internal class InputComponentXmlReader : IComponentXmlReader
    {
        public string NodeName
        {
            get { return "Input"; }
        }

        public IComponentInfo Load(XElement node, Project project)
        {
            return new InputComponentInfo();
        }
    }
}
