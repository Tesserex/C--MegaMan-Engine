using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class EffectXmlReader
    {
        internal EffectInfo Load(XElement effectNode)
        {
            var info = new EffectInfo();
            info.Name = effectNode.TryAttribute<string>("name");

            var parts = new List<IEffectPartInfo>();

            foreach (var node in effectNode.Elements())
                parts.Add(LoadPart(node));

            return info;
        }

        private IEffectPartInfo LoadPart(XElement node)
        {
            if (!PartReaders.ContainsKey(node.Name.LocalName))
                throw new GameXmlException(node, "Unrecognized effect part.");

            var reader = PartReaders[node.Name.LocalName];

            return reader.Load(node);
        }

        private static Dictionary<string, IEffectPartXmlReader> PartReaders;

        static EffectXmlReader()
        {
            PartReaders = Extensions.GetImplementersOf<IEffectPartXmlReader>()
                .ToDictionary(x => x.NodeName);
        }
    }
}
