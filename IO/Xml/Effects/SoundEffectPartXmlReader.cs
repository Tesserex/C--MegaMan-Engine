using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

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
            return new SoundEffectPartInfo {
                Name = partNode.GetAttribute<string>("name"),
                Playing = partNode.TryAttribute("playing", true)
            };
        }
    }
}
