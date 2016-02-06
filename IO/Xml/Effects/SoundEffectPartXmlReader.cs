using System;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Effects
{
    internal class SoundEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Sound";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new SoundEffectPartInfo() {
                Name = partNode.GetAttribute<string>("name"),
                Playing = partNode.TryAttribute<bool>("playing", true)
            };
        }
    }
}
