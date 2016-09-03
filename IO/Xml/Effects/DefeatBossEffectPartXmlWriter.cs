using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class DefeatBossEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(DefeatBossEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("DefeatBoss");
            writer.WriteAttributeString("name", ((DefeatBossEffectPartInfo)info).Name);
            writer.WriteEndElement();
        }
    }
}
