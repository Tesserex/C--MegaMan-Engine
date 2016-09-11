using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class EffectXmlWriter
    {
        public void Write(EffectInfo effect, XmlWriter writer)
        {
            writer.WriteStartElement("Effect");
            WriteEffectContents(effect, writer);
            writer.WriteEndElement();
        }

        public void WriteElse(EffectInfo effect, XmlWriter writer)
        {
            writer.WriteStartElement("Else");
            WriteEffectContents(effect, writer);
            writer.WriteEndElement();
        }

        private void WriteEffectContents(EffectInfo effect, XmlWriter writer)
        {
            if (!string.IsNullOrWhiteSpace(effect.Name))
                writer.WriteAttributeString("name", effect.Name);

            if (effect.Filter != null)
            {
                writer.WriteStartElement("EntityFilter");

                if (effect.Filter.Type != null)
                    writer.WriteElementString("Type", effect.Filter.Type);

                if (effect.Filter.Direction != null)
                    writer.WriteElementString("Direction", effect.Filter.Direction.ToString());

                if (effect.Filter.Position != null)
                {
                    if (effect.Filter.Position.X != null)
                        WriteRangeFilter("X", writer, effect.Filter.Position.X);

                    if (effect.Filter.Position.Y != null)
                        WriteRangeFilter("Y", writer, effect.Filter.Position.Y);
                }

                writer.WriteEndElement();
            }

            foreach (var part in effect.Parts)
                WritePart(part, writer);
        }

        public void WriteRangeFilter(string tag, XmlWriter writer, RangeFilter filter)
        {
            writer.WriteStartElement(tag);
            if (filter.Min.HasValue)
                writer.WriteAttributeString("min", filter.Min.Value.ToString());

            if (filter.Max.HasValue)
                writer.WriteAttributeString("max", filter.Max.Value.ToString());

            writer.WriteEndElement();
        }

        public void WritePart(IEffectPartInfo info, XmlWriter writer)
        {
            if (!PartWriters.ContainsKey(info.GetType()))
                throw new Exception("No xml writer for effect part type: " + info.GetType().Name);

            var compWriter = PartWriters[info.GetType()];

            compWriter.Write(info, writer);
        }

        private static Dictionary<Type, IEffectPartXmlWriter> PartWriters;

        static EffectXmlWriter()
        {
            PartWriters = Extensions.GetImplementersOf<IEffectPartXmlWriter>()
                .ToDictionary(x => x.EffectPartType);
        }
    }
}
