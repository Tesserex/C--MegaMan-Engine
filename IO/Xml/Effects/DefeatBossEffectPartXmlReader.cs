using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class DefeatBossEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "DefeatBoss";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new DefeatBossEffectPartInfo {
                Name = partNode.GetAttribute<string>("name")
            };
        }
    }
}
