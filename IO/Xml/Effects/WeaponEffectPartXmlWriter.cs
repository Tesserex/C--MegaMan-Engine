using System;
using System.Xml;
using MegaMan.Common.Entities.Effects;

namespace MegaMan.IO.Xml.Effects
{
    internal class WeaponEffectPartXmlWriter : IEffectPartXmlWriter
    {
        public Type EffectPartType
        {
            get
            {
                return typeof(WeaponEffectPartInfo);
            }
        }

        public void Write(IEffectPartInfo info, XmlWriter writer)
        {
            var weapon = (WeaponEffectPartInfo)info;

            writer.WriteStartElement("Weapon");

            switch (weapon.Action)
            {
                case WeaponAction.Shoot:
                case WeaponAction.RotateForward:
                case WeaponAction.RotateBackward:
                    writer.WriteValue(weapon.Action.ToString());
                    break;

                case WeaponAction.Ammo:
                    writer.WriteAttributeString("val", weapon.Ammo.ToString());
                    break;

                case WeaponAction.Change:
                    writer.WriteAttributeString("name", weapon.ChangeName);
                    break;
            }

            writer.WriteEndElement();
        }
    }
}
