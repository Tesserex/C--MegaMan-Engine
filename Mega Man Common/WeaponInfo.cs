using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public class PauseWeaponInfo
    {
        public FilePath IconOn { get; set; }
        public FilePath IconOff { get; set; }
        public string Name { get; set; }
        public string Weapon { get; set; }
        public Point Location { get; set; }
        public MeterInfo Meter { get; set; }

        public static PauseWeaponInfo FromXml(XElement weaponNode, string basePath)
        {
            PauseWeaponInfo info = new PauseWeaponInfo();
            info.Name = weaponNode.RequireAttribute("name").Value;
            info.Weapon = weaponNode.RequireAttribute("weapon").Value;

            info.IconOn = FilePath.FromRelative(weaponNode.RequireAttribute("on").Value, basePath);
            info.IconOff = FilePath.FromRelative(weaponNode.RequireAttribute("off").Value, basePath);

            info.Location = new Point(weaponNode.GetInteger("x"), weaponNode.GetInteger("y"));

            XElement meter = weaponNode.Element("Meter");
            if (meter != null)
            {
                info.Meter = MeterInfo.FromXml(meter, basePath);
            }

            return info;
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Weapon");

            writer.WriteAttributeString("name", Name);
            writer.WriteAttributeString("weapon", Weapon);
            writer.WriteAttributeString("on", IconOn.Relative);
            writer.WriteAttributeString("off", IconOff.Relative);
            writer.WriteAttributeString("x", Location.X.ToString());
            writer.WriteAttributeString("y", Location.Y.ToString());

            if (Meter != null) Meter.Save(writer);

            writer.WriteEndElement();
        }
    }
}
