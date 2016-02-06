using System;
using System.Xml.Linq;
using System.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Effects
{
    internal class DelayEffectPartXmlReader : IEffectPartXmlReader
    {
        private readonly EffectXmlReader _effectReader;

        public DelayEffectPartXmlReader(EffectXmlReader effectReader)
        {
            _effectReader = effectReader;
        }

        public string NodeName
        {
            get
            {
                return "Delay";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new DelayedEffectPartInfo() {
                DelayFrames = partNode.GetAttribute<int>("frames"),
                Effect = _effectReader.Load(partNode)
            };
        }
    }
}
