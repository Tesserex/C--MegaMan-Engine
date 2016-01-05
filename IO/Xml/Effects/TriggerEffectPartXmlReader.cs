using System;
using System.Xml.Linq;
using System.Linq;
using MegaMan.Common.Entities.Effects;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Effects
{
    internal class TriggerEffectPartXmlReader : IEffectPartXmlReader
    {
        private readonly TriggerXmlReader _triggerReader;

        public TriggerEffectPartXmlReader(TriggerXmlReader triggerReader)
        {
            _triggerReader = triggerReader;
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
                Trigger = _triggerReader.Load(partNode)
            };
        }
    }
}
