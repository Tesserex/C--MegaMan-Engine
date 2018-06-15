using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class TriggerEffectPartXmlWriter : IEffectPartXmlWriter
    {
        private readonly TriggerXmlWriter triggerWriter;

        public TriggerEffectPartXmlWriter(TriggerXmlWriter triggerWriter)
        {
            this.triggerWriter = triggerWriter;
        }

        public Type EffectPartType
        {
            get
            {
                return typeof(TriggerEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            triggerWriter.Write(((TriggerEffectPartInfo)info).Trigger, writer);
        }
    }
}
