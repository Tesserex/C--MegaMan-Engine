using System.Xml;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Effects
{
    internal class TriggerXmlWriter
    {
        private EffectXmlWriter effectXmlWriter;

        public TriggerXmlWriter(EffectXmlWriter effectWriter)
        {
            effectXmlWriter = effectWriter;
        }
        
        internal void Write(TriggerInfo trigger, XmlWriter writer)
        {
            writer.WriteStartElement("Trigger");
            writer.WriteAttributeString("priority", trigger.Priority.ToString());

            writer.WriteElementString("Condition", trigger.Condition);
            effectXmlWriter.Write(trigger.Effect, writer);

            if (trigger.Else != null)
                effectXmlWriter.WriteElse(trigger.Else, writer);

            writer.WriteEndElement();
        }

        public void WriteMulti(MultiStateTriggerInfo trigger, XmlWriter writer)
        {
            writer.WriteStartElement("Trigger");
            writer.WriteAttributeString("priority", trigger.Trigger.Priority.ToString());

            if (trigger.States != null)
            {
                var states = string.Join(",", trigger.States);
                writer.WriteElementString("States", states);
            }

            writer.WriteElementString("Condition", trigger.Trigger.Condition);
            effectXmlWriter.Write(trigger.Trigger.Effect, writer);

            if (trigger.Trigger.Else != null)
                effectXmlWriter.WriteElse(trigger.Trigger.Else, writer);

            writer.WriteEndElement();
        }
    }
}
