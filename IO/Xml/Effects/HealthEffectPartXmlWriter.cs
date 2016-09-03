using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class HealthEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(HealthEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var health = (HealthEffectPartInfo)info;
            writer.WriteStartElement("Health");
            writer.WriteAttributeString("change", health.Change.ToString());
            writer.WriteEndElement();
        }
    }
}
