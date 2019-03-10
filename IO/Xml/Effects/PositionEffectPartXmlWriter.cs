using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class PositionEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(PositionEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var pos = (PositionEffectPartInfo)info;

            if (pos == null) return;

            writer.WriteStartElement("Position");

            if (pos.X != null)
            {
                writer.WriteStartElement("X");
                WriteAxis(pos.X, writer);
                writer.WriteEndElement();
            }

            if (pos.Y != null)
            {
                writer.WriteStartElement("Y");
                WriteAxis(pos.Y, writer);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        private void WriteAxis(PositionEffectAxisInfo axis, XmlWriter writer)
        {
            if (axis.Base.HasValue)
                writer.WriteAttributeString("base", axis.Base.Value.ToString());
            else
                writer.WriteAttributeString("base", "Inherit");

            if (axis.Offset.HasValue)
            {
                writer.WriteAttributeString("offset", axis.Offset.Value.ToString());
                writer.WriteAttributeString("direction", axis.OffsetDirection.ToString());
            }
        }
    }
}
