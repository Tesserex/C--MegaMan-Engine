using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Includes
{
    internal class EntitiesXmlReader : IIncludeXmlReader
    {
        private EntityXmlReader entityReader;

        public EntitiesXmlReader(EntityXmlReader entityReader)
        {
            this.entityReader = entityReader;
        }

        public string NodeName
        {
            get { return "Entities"; }
        }

        public void Load(Project project, XElement xmlNode)
        {
            foreach (var node in xmlNode.Elements("Entity"))
            {
                this.entityReader.Load(project, node);
            }
        }
    }
}
