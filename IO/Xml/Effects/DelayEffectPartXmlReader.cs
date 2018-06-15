using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class DelayEffectPartXmlReader : IEffectPartXmlReader
    {
        private readonly EffectXmlReader effectReader;

        public DelayEffectPartXmlReader(EffectXmlReader effectReader)
        {
            this.effectReader = effectReader;
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
            return new DelayedEffectPartInfo {
                DelayFrames = partNode.GetAttribute<int>("frames"),
                Effect = effectReader.Load(partNode)
            };
        }
    }
}
