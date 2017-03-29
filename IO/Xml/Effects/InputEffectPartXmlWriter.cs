using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class InputEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(InputEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var input = (InputEffectPartInfo)info;
            writer.WriteStartElement("Input");

            if (input.Paused)
                writer.WriteElementString("Pause", "");
            else
                writer.WriteElementString("Unpause", "");

            writer.WriteEndElement();
        }
    }
}
