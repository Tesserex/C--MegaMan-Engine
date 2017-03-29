using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class RemoveEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(RemoveEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            writer.WriteElementString("Remove", "");
        }
    }
}
