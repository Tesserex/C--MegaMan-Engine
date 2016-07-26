using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class EffectXmlReader
    {
        internal EffectInfo Load(XElement effectNode)
        {
            var info = new EffectInfo();
            info.Name = effectNode.TryAttribute<string>("name");

            var filterNode = effectNode.Element("EntityFilter");
            if (filterNode != null)
            {
                info.Filter = new EntityFilterInfo() {
                    Type = filterNode.TryElementValue<string>("Type")
                };
            }

            var parts = new List<IEffectPartInfo>();

            foreach (var node in effectNode.Elements().Where(e => e.Name != "EntityFilter"))
                parts.Add(LoadPart(node));

            info.Parts = parts;

            return info;
        }

        public IEffectPartInfo LoadPart(XElement node)
        {
            if (!PartReaders.ContainsKey(node.Name.LocalName))
                throw new GameXmlException(node, "Unrecognized effect part: " + node.Name.LocalName);

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
