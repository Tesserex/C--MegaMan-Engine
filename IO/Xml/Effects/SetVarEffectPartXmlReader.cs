using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class SetVarEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "SetVar";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new SetVarEffectPartInfo() {
                Name = partNode.GetAttribute<string>("name"),
                Value = partNode.GetAttribute<string>("value")
            };
        }
    }
}
