using System;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Effects
{
    internal class GravityFlipEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "GravityFlip";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new GravityFlipEffectPartInfo() {
                Flipped = partNode.GetValue<bool>()
            };
        }
    }
}
