using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class HealthEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Health";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new HealthEffectPartInfo {
                Change = partNode.GetAttribute<float>("change")
            };
        }
    }
}
