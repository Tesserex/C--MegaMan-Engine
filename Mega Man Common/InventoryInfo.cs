using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public class InventoryInfo
    {
        public FilePath IconOn { get; set; }
        public FilePath IconOff { get; set; }
        public string Name { get; set; }
        public string UseFunction { get; set; }
        public Point IconLocation { get; set; }
        public Point NumberLocation { get; set; }
        public bool Selectable { get; set; }

        public static InventoryInfo FromXml(XElement inventoryNode, string basePath)
        {
            InventoryInfo info = new InventoryInfo();
            info.Name = inventoryNode.RequireAttribute("name").Value;

            var useAttr = inventoryNode.Attribute("use");
            if (useAttr != null)
            {
                info.UseFunction = useAttr.Value;
            }

            var iconNode = inventoryNode.Element("Icon");
            if (iconNode != null)
            {
                info.IconOn = FilePath.FromRelative(iconNode.RequireAttribute("on").Value, basePath);
                info.IconOff = FilePath.FromRelative(iconNode.RequireAttribute("off").Value, basePath);
            }

            info.IconLocation = new Point(iconNode.GetInteger("x"), iconNode.GetInteger("y"));

            var numberNode = inventoryNode.Element("Number");
            if (numberNode != null)
            {
                info.NumberLocation = new Point(numberNode.GetInteger("x"), numberNode.GetInteger("y"));
            }

            bool b = true;
            inventoryNode.TryBool("selectable", out b);
            info.Selectable = b;

            return info;
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Inventory");

            writer.WriteAttributeString("name", Name);
            if (UseFunction != null) writer.WriteAttributeString("use", UseFunction);
            writer.WriteAttributeString("selectable", Selectable.ToString());

            if (IconOn != null)
            {
                writer.WriteStartElement("Icon");
                writer.WriteAttributeString("on", IconOn.Relative);
                writer.WriteAttributeString("off", IconOff.Relative);
                writer.WriteAttributeString("x", IconLocation.X.ToString());
                writer.WriteAttributeString("y", IconLocation.Y.ToString());
                writer.WriteEndElement();
            }

            if (NumberLocation != Point.Empty)
            {
                writer.WriteStartElement("Number");
                writer.WriteAttributeString("x", NumberLocation.X.ToString());
                writer.WriteAttributeString("y", NumberLocation.Y.ToString());
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
