using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class LivesEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Lives";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new LivesEffectPartInfo() {
                Add = partNode.GetAttribute<int>("add")
            };
        }
    }
}
