using System;
using System.Xml;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Entities
{
    internal class LadderComponentXmlWriter : IComponentXmlWriter
    {
        public Type ComponentType
        {
            get
            {
                return typeof(LadderComponentInfo);
            }
        }

        public void Write(IComponentInfo info, XmlWriter writer)
        {
            var ladder = (LadderComponentInfo)info;

            writer.WriteStartElement("Ladder");

            foreach (var hitbox in ladder.HitBoxes)
                WriteHitbox(hitbox, writer);

            writer.WriteEndElement();
        }

        private void WriteHitbox(HitBoxInfo box, XmlWriter writer)
        {
            writer.WriteStartElement("Hitbox");

            if (box.Name != null)
                writer.WriteAttributeString("name", box.Name);

            writer.WriteAttributeString("x", box.Box.X.ToString());
            writer.WriteAttributeString("y", box.Box.Y.ToString());
            writer.WriteAttributeString("width", box.Box.Width.ToString());
            writer.WriteAttributeString("height", box.Box.Height.ToString());

            if (box.PropertiesName != null)
                writer.WriteAttributeString("properties", box.PropertiesName);

            if (box.ContactDamage != 0)
                writer.WriteAttributeString("damage", box.ContactDamage.ToString());

            writer.WriteAttributeString("environment", box.Environment.ToString());
            writer.WriteAttributeString("pushaway", box.PushAway.ToString());

            foreach (var hit in box.Hits)
                writer.WriteElementString("Hits", hit);

            foreach (var group in box.Groups)
                writer.WriteElementString("Group", group);

            foreach (var resist in box.Resistance)
            {
                writer.WriteStartElement("Resist");
                writer.WriteAttributeString("name", resist.Key);
                writer.WriteAttributeString("multiply", resist.Value.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
