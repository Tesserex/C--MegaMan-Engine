using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class PaletteEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(PaletteEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var pal = (PaletteEffectPartInfo)info;
            writer.WriteStartElement("Palette");
            writer.WriteAttributeString("name", pal.PaletteName);
            writer.WriteAttributeString("index", pal.PaletteIndex.ToString());
            writer.WriteEndElement();
        }
    }
}
