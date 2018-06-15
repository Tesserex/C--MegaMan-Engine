using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Includes
{
    internal class EntitiesXmlReader : IIncludeXmlReader
    {
        private readonly EntityXmlReader entityReader;
        private readonly TilesetXmlReader tilesetReader;

        public EntitiesXmlReader(EntityXmlReader entityReader, TilesetXmlReader tilesetReader)
        {
            this.entityReader = entityReader;
            this.tilesetReader = tilesetReader;
        }

        public string NodeName
        {
            get { return "Entities"; }
        }

        public IIncludedObject Load(Project project, XElement xmlNode, IDataSource dataSource)
        {
            LoadProperties(project, xmlNode);

            var group = new IncludedObjectGroup();
            foreach (var node in xmlNode.Elements("Entity"))
            {
                group.Add(entityReader.Load(project, node, dataSource));
            }

            return group;
        }

        private void LoadProperties(Project project, XElement node)
        {
            var propHead = node.Element("Properties");
            if (propHead != null)
            {
                foreach (var propNode in propHead.Elements("Properties"))
                {
                    var properties = tilesetReader.LoadProperties(propNode);
                    project.AddEntityProperties(properties);
                }
            }
        }
    }
}
