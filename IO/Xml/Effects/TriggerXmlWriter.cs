using System.Xml;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Effects
{
    internal class TriggerXmlWriter
    {
        private EffectXmlWriter _effectXmlWriter;
        
        internal void Write(TriggerInfo trigger, XmlWriter writer)
        {
            writer.WriteStartElement("Trigger");
            writer.WriteElementString("Condition", trigger.Condition);

            _effectXmlWriter.Write(trigger.Effect, writer);

            writer.WriteEndElement();
        }

        public void WriteMulti(MultiStateTriggerInfo trigger, XmlWriter writer)
        {
            writer.WriteStartElement("Trigger");

            var states = string.Join(",", trigger.States);
            writer.WriteElementString("States", states);
            writer.WriteElementString("Condition", trigger.Trigger.Condition);
            _effectXmlWriter.Write(trigger.Trigger.Effect, writer);

            writer.WriteEndElement();
        }
    }
}
