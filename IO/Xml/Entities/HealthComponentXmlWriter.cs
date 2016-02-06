using System;
using System.Xml;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Entities
{
    internal class HealthComponentXmlWriter : IComponentXmlWriter
    {
        private readonly MeterXmlWriter _meterWriter;

        public HealthComponentXmlWriter(MeterXmlWriter meterWriter)
        {
            _meterWriter = meterWriter;
        }

        public Type ComponentType
        {
            get { return typeof(HealthComponentInfo); }
        }

        public void Write(IComponentInfo info, XmlWriter writer)
        {
            var health = (HealthComponentInfo)info;

            writer.WriteStartElement("Health");
            writer.WriteAttributeString("max", health.Max.ToString());

            if (health.StartValue != null)
                writer.WriteAttributeString("startValue", health.StartValue.ToString());

            writer.WriteAttributeString("flash", health.FlashFrames.ToString());

            if (health.Meter != null)
                _meterWriter.Write(health.Meter, writer);

            writer.WriteEndElement();
        }
    }
}
