using System;
using System.Xml;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.Xml.Handlers;

namespace MegaMan.IO.Xml
{
    internal class MeterXmlWriter : IHandlerObjectXmlWriter
    {
        private readonly SoundXmlWriter _soundWriter;
        private readonly SceneBindingXmlWriter _bindingWriter;

        public MeterXmlWriter(SoundXmlWriter soundWriter, SceneBindingXmlWriter bindingWriter)
        {
            _soundWriter = soundWriter;
            _bindingWriter = bindingWriter;
        }

        public void Write(IHandlerObjectInfo info, XmlWriter writer)
        {
            var meter = (MeterInfo)info;

            writer.WriteStartElement("Meter");
            writer.WriteAttributeString("name", meter.Name);
            writer.WriteAttributeString("x", meter.Position.X.ToString());
            writer.WriteAttributeString("y", meter.Position.Y.ToString());
            writer.WriteAttributeString("image", meter.TickImage.Relative);

            if (meter.Background != null)
                writer.WriteAttributeString("background", meter.Background.Relative);

            writer.WriteAttributeString("orientation", meter.Orient.ToString().ToLower());

            writer.WriteAttributeString("tickX", meter.TickOffset.X.ToString());
            writer.WriteAttributeString("tickY", meter.TickOffset.Y.ToString());

            if (meter.Sound != null)
                _soundWriter.Write(meter.Sound, writer);

            if (meter.Binding != null)
                _bindingWriter.Write(meter.Binding, writer);

            writer.WriteEndElement();
        }

        public Type ObjectType
        {
            get { return typeof(MeterInfo); }
        }
    }
}
