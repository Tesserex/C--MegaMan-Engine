using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class SoundEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(SoundEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var sound = (SoundEffectPartInfo)info;
            writer.WriteStartElement("Sound");
            writer.WriteAttributeString("name", sound.Name);
            writer.WriteAttributeString("playing", sound.Playing.ToString());
            writer.WriteEndElement();
        }
    }
}
