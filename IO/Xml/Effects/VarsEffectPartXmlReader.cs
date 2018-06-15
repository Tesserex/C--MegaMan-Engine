using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class VarsEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Vars";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new VarsEffectPartInfo {
                Name = partNode.GetAttribute<string>("name"),
                Value = partNode.TryAttribute<string>("value"),
                Call = partNode.TryAttribute<string>("call"),
                EntityName = partNode.TryAttribute<string>("entity")
            };
        }
    }
}
