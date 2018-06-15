using System;
using System.Xml;
using MegaMan.Common.Entities;

namespace MegaMan.IO.Xml.Entities
{
    internal class WeaponComponentXmlWriter : IComponentXmlWriter
    {
        private readonly MeterXmlWriter meterWriter;

        public WeaponComponentXmlWriter(MeterXmlWriter meterWriter)
        {
            this.meterWriter = meterWriter;
        }

        public Type ComponentType
        {
            get
            {
                return typeof(WeaponComponentInfo);
            }
        }

        public void Write(IComponentInfo info, XmlWriter writer)
        {
            var comp = (WeaponComponentInfo)info;

            writer.WriteStartElement("Weapons");

            foreach (var weapon in comp.Weapons)
            {
                writer.WriteStartElement("Weapon");

                writer.WriteAttributeString("name", weapon.Name);
                writer.WriteAttributeString("entity", weapon.EntityName);

                if (weapon.Ammo.HasValue)
                    writer.WriteAttributeString("ammo", weapon.Ammo.Value.ToString());

                if (weapon.Usage.HasValue)
                    writer.WriteAttributeString("usage", weapon.Usage.Value.ToString());

                if (weapon.Palette.HasValue)
                    writer.WriteAttributeString("palette", weapon.Palette.Value.ToString());

                if (weapon.Meter != null)
                    meterWriter.Write(weapon.Meter, writer);

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
