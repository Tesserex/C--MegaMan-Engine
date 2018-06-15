using System;
using System.Xml;
using MegaMan.Common;
using MegaMan.IO.Xml.Effects;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class EffectCommandXmlWriter : ICommandXmlWriter
    {
        private readonly EffectXmlWriter effectWriter;

        public EffectCommandXmlWriter(EffectXmlWriter effectWriter)
        {
            this.effectWriter = effectWriter;
        }

        public Type CommandType
        {
            get
            {
                return typeof(SceneEffectCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var effect = (SceneEffectCommandInfo)info;

            writer.WriteStartElement("Effect");
            if (effect.EntityId != null)
                writer.WriteAttributeString("entity", effect.EntityId);

            foreach (var part in effect.EffectInfo.Parts)
                effectWriter.WritePart(part, writer);

            writer.WriteEndElement();
        }
    }
}
