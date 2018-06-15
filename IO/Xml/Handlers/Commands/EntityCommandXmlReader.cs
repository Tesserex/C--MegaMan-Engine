using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class EntityCommandXmlReader : ICommandXmlReader
    {
        private readonly EntityPlacementXmlReader entityReader;

        public EntityCommandXmlReader(EntityPlacementXmlReader entityReader)
        {
            this.entityReader = entityReader;
        }

        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Entity";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneEntityCommandInfo();
            info.Placement = entityReader.Load(node);
            return info;
        }
    }
}
