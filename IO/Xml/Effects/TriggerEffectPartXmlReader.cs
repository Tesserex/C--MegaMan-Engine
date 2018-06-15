using System.Xml.Linq;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class TriggerEffectPartXmlReader : IEffectPartXmlReader
    {
        private readonly TriggerXmlReader triggerReader;

        public TriggerEffectPartXmlReader(TriggerXmlReader triggerReader)
        {
            this.triggerReader = triggerReader;
        }

        public string NodeName
        {
            get
            {
                return "Trigger";
            }
        }

        public IEffectPartInfo Load(XElement partNode)
        {
            return new TriggerEffectPartInfo() {
                Trigger = triggerReader.Load(partNode)
            };
        }
    }
}
