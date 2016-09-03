using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class SpriteEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(SpriteEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var sprite = (SpriteEffectPartInfo)info;

            writer.WriteStartElement("Sprite");
            writer.WriteElementString("Name", sprite.Name);

            if (sprite.Playing != null)
                writer.WriteElementString("Playing", sprite.Playing.Value.ToString());

            if (sprite.Visible != null)
                writer.WriteElementString("Visible", sprite.Visible.Value.ToString());

            writer.WriteEndElement();
        }
    }
}
