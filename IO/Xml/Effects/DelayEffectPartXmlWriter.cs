using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class DelayEffectPartXmlWriter : IEffectPartXmlWriter
    {
        private readonly EffectXmlWriter effectWriter;

        public DelayEffectPartXmlWriter(EffectXmlWriter effectWriter)
        {
            this.effectWriter = effectWriter;
        }

        public Type EffectPartType
        {
            get
            {
                return typeof(DelayedEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var delay = (DelayedEffectPartInfo)info;
            writer.WriteStartElement("Delay");

            foreach (var part in delay.Effect.Parts)
                effectWriter.WritePart(part, writer);

            writer.WriteEndElement();
        }
    }
}
