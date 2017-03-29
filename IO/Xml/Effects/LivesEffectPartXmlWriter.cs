using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class LivesEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(LivesEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var lives = (LivesEffectPartInfo)info;
            writer.WriteStartElement("Lives");
            writer.WriteAttributeString("add", lives.Add.ToString());
            writer.WriteEndElement();
        }
    }
}
