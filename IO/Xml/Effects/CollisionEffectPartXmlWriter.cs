using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class CollisionEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(CollisionEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var collision = (CollisionEffectPartInfo)info;
            writer.WriteStartElement("Collision");

            if (collision.ClearEnabled)
                writer.WriteElementString("Clear", "");

            if (collision.Enabled.HasValue)
                writer.WriteElementString("Enabled", collision.Enabled.Value.ToString());

            foreach (var hitbox in collision.HitBoxes)
            {
                writer.WriteStartElement("Hitbox");

                writer.WriteAttributeString("x", hitbox.Box.X.ToString());
                writer.WriteAttributeString("y", hitbox.Box.Y.ToString());
                writer.WriteAttributeString("width", hitbox.Box.Width.ToString());
                writer.WriteAttributeString("height", hitbox.Box.Height.ToString());
                writer.WriteAttributeString("damage", hitbox.ContactDamage.ToString());
                writer.WriteAttributeString("environment", hitbox.Environment.ToString());
                writer.WriteAttributeString("pushaway", hitbox.PushAway.ToString());
                writer.WriteAttributeString("properties", hitbox.PropertiesName);

                foreach (var group in hitbox.Groups)
                    writer.WriteElementString("Group", group);

                foreach (var hits in hitbox.Hits)
                    writer.WriteElementString("Hits", hits);

                foreach (var resist in hitbox.Resistance)
                {
                    writer.WriteStartElement("Resist");
                    writer.WriteAttributeString("name", resist.Key);
                    writer.WriteAttributeString("multiply", resist.Value.ToString());
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }

            foreach (var enable in collision.EnabledBoxes)
            {
                writer.WriteStartElement("EnableBox");
                writer.WriteAttributeString("name", enable);
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
