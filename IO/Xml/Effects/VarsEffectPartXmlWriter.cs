using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class VarsEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(VarsEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var set = (VarsEffectPartInfo)info;
            writer.WriteStartElement("Vars");
            writer.WriteAttributeString("name", set.Name);
            writer.WriteAttributeString("value", set.Value);
            writer.WriteEndElement();
        }
    }
}
