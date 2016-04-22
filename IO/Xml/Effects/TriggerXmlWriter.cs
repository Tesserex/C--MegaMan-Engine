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
    }
}
