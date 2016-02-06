using System;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class CallEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Call";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new CallEffectPartInfo() {
                EffectName = partNode.Value
            };
        }
    }
}
