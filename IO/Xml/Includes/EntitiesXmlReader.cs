using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO.Xml.Includes
{
    internal class EntitiesXmlReader : IIncludeXmlReader
    {
        private readonly EntityXmlReader _entityReader;
        private readonly TilesetXmlReader _tilesetReader;

        public EntitiesXmlReader(EntityXmlReader entityReader, TilesetXmlReader tilesetReader)
        {
            this._entityReader = entityReader;
            _tilesetReader = tilesetReader;
        }

        public string NodeName
        {
            get { return "Entities"; }
        }

        public IIncludedObject Load(Project project, XElement xmlNode)
        {
            LoadProperties(project, xmlNode);

            var group = new IncludedObjectGroup();
            foreach (var node in xmlNode.Elements("Entity"))
            {
                group.Add(this._entityReader.Load(project, node));
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
                    var properties = _tilesetReader.LoadProperties(propNode);
                    project.AddEntityProperties(properties);
                }
            }
        }
    }
}
