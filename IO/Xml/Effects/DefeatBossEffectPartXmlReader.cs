using System;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

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
            return new DefeatBossEffectPartInfo() {
                Name = partNode.GetAttribute<string>("name")
            };
        }
    }
}
