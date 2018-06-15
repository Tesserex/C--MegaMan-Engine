using System.Linq;
using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class InputEffectPartXmlReader : IEffectPartXmlReader
    {
        public string NodeName
        {
            get
            {
                return "Input";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new InputEffectPartInfo() {
                Paused = partNode.Elements("Pause").Any()
            };
        }
    }
}
