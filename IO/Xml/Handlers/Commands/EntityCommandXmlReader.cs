using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class EntityCommandXmlReader : ICommandXmlReader
    {
        private readonly EntityPlacementXmlReader _entityReader;

        public EntityCommandXmlReader(EntityPlacementXmlReader entityReader)
        {
            _entityReader = entityReader;
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
            info.Placement = _entityReader.Load(node);
            return info;
        }
    }
}
