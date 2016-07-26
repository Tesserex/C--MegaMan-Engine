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

            if (!string.IsNullOrWhiteSpace(effect.Name))
                writer.WriteAttributeString("name", effect.Name);

            if (effect.Filter != null)
            {
                writer.WriteStartElement("EntityFilter");

                if (effect.Filter.Type != null)
                    writer.WriteElementString("Type", effect.Filter.Type);

                writer.WriteEndElement();
            }

            foreach (var part in effect.Parts)
                WritePart(part, writer);

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
