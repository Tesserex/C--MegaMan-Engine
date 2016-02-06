using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml
{
    internal class SpriteXmlWriter
    {
        public void Write(Sprite sprite, XmlWriter writer)
        {
            writer.WriteStartElement("Sprite");

            if (sprite.PaletteName != null)
                writer.WriteAttributeString("palette", sprite.PaletteName);

            writer.WriteAttributeString("width", sprite.Width.ToString());
            writer.WriteAttributeString("height", sprite.Height.ToString());

            if (sprite.SheetPathRelative != null)
                writer.WriteAttributeString("tilesheet", sprite.SheetPathRelative);

            writer.WriteStartElement("Hotspot");
            writer.WriteAttributeString("x", sprite.HotSpot.X.ToString());
            writer.WriteAttributeString("y", sprite.HotSpot.Y.ToString());
            writer.WriteEndElement();

            foreach (SpriteFrame frame in sprite)
            {
                writer.WriteStartElement("Frame");
                writer.WriteAttributeString("x", frame.SheetLocation.X.ToString());
                writer.WriteAttributeString("y", frame.SheetLocation.Y.ToString());
                writer.WriteAttributeString("duration", frame.Duration.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
