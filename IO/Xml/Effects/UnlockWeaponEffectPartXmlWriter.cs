using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class UnlockWeaponEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(UnlockWeaponEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("UnlockWeapon");
            writer.WriteAttributeString("name", ((UnlockWeaponEffectPartInfo)info).WeaponName);
            writer.WriteEndElement();
        }
    }
}
