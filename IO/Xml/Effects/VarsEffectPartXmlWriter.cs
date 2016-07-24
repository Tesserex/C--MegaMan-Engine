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

            if (set.Value != null)
                writer.WriteAttributeString("value", set.Value);

            if (set.Call != null)
                writer.WriteAttributeString("call", set.Call);

            if (set.EntityName != null)
                writer.WriteAttributeString("entity", set.EntityName);

            writer.WriteEndElement();
        }
    }
}
