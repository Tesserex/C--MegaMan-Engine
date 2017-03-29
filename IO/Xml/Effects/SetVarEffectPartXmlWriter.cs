using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class SetVarEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(SetVarEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var set = (SetVarEffectPartInfo)info;
            writer.WriteStartElement("SetVar");
            writer.WriteAttributeString("name", set.Name);
            writer.WriteAttributeString("value", set.Value);
            writer.WriteEndElement();
        }
    }
}
