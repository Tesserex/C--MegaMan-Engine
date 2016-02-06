using System.Xml.Linq;
using MegaMan.Common;

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

        public void Load(Project project, XElement xmlNode)
        {
            LoadProperties(project, xmlNode);

            foreach (var node in xmlNode.Elements("Entity"))
            {
                this._entityReader.Load(project, node);
            }
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
