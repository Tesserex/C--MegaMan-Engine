using System.Collections.Generic;
using System.Drawing;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public class PauseScreen
    {
        private List<PauseWeaponInfo> weapons;

        public IEnumerable<PauseWeaponInfo> Weapons
        {
            get { return weapons; }
        }

        private List<InventoryInfo> inventory;

        public IEnumerable<InventoryInfo> Inventory
        {
            get { return inventory; }
        }

        public FilePath Background { get; set; }

        public SoundInfo ChangeSound { get; set; }

        public SoundInfo PauseSound { get; set; }

        public Point LivesPosition { get; set; }

        public PauseScreen(XElement reader, string basePath)
        {
            weapons = new List<PauseWeaponInfo>();
            inventory = new List<InventoryInfo>();

            XElement changeNode = reader.Element("ChangeSound");
            if (changeNode != null) ChangeSound = SoundInfo.FromXml(changeNode, basePath);

            XElement soundNode = reader.Element("PauseSound");
            if (soundNode != null) PauseSound = SoundInfo.FromXml(soundNode, basePath);

            XElement backgroundNode = reader.Element("Background");
            if (backgroundNode != null) Background = FilePath.FromRelative(backgroundNode.Value, basePath);

            foreach (XElement weapon in reader.Elements("Weapon"))
                weapons.Add(PauseWeaponInfo.FromXml(weapon, basePath));

            XElement livesNode = reader.Element("Lives");
            if (livesNode != null)
            {
                LivesPosition = new Point(livesNode.GetInteger("x"), livesNode.GetInteger("y"));
            }

            foreach (XElement inventoryNode in reader.Elements("Inventory"))
            {
                inventory.Add(InventoryInfo.FromXml(inventoryNode, basePath));
            }
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("PauseScreen");

            if (ChangeSound != null) ChangeSound.Save(writer);
            if (PauseSound != null) PauseSound.Save(writer);

            if (Background != null) writer.WriteElementString("Background", Background.Relative);

            foreach (PauseWeaponInfo weapon in weapons) weapon.Save(writer);

            if (LivesPosition != Point.Empty)
            {
                writer.WriteStartElement("Lives");
                writer.WriteAttributeString("x", LivesPosition.X.ToString());
                writer.WriteAttributeString("y", LivesPosition.Y.ToString());
                writer.WriteEndElement();
            }

            foreach (var item in inventory)
            {
                item.Save(writer);
            }

            writer.WriteEndElement();
        }
    }
}
