using System;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class LadderEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Ladder";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new LadderEffectPartInfo {
                Action = (LadderAction)Enum.Parse(typeof(LadderAction), partNode.Value)
            };
        }
    }
}
