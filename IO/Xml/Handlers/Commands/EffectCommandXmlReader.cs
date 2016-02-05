using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.IO.Xml.Effects;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class EffectCommandXmlReader : ICommandXmlReader
    {
        private readonly EffectXmlReader _effectReader;

        public EffectCommandXmlReader(EffectXmlReader effectReader)
        {
            _effectReader = effectReader;
        }

        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Effect";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneEffectCommandInfo();

            info.GeneratedName = Guid.NewGuid().ToString();

            var attr = node.Attribute("entity");
            if (attr != null)
            {
                info.EntityId = attr.Value;
            }
            
            info.EffectInfo = _effectReader.Load(node);

            return info;
        }
    }
}
