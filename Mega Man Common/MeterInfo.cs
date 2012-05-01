using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Drawing;
using System.Xml;

namespace MegaMan.Common
{
    public class MeterInfo : IHandlerObjectInfo
    {
        public enum Orientation : byte
        {
            Horizontal,
            Vertical
        }

        public string Name { get; set; }
        public PointF Position { get; set; }
        public FilePath Background { get; set; }
        public FilePath TickImage { get; set; }
        public Orientation Orient { get; set; }
        public Point TickOffset { get; set; }
        public SoundInfo Sound { get; set; }
        public SceneBindingInfo Binding { get; set; }

        public static MeterInfo FromXml(XElement meterNode, string basePath)
        {
            MeterInfo meter = new MeterInfo();

            meter.Name = meterNode.RequireAttribute("name").Value;

            meter.Position = new PointF(meterNode.GetFloat("x"), meterNode.GetFloat("y"));

            XAttribute imageAttr = meterNode.RequireAttribute("image");
            meter.TickImage = FilePath.FromRelative(imageAttr.Value, basePath);

            XAttribute backAttr = meterNode.Attribute("background");
            if (backAttr != null)
            {
                meter.Background = FilePath.FromRelative(backAttr.Value, basePath);
            }

            bool horiz = false;
            XAttribute dirAttr = meterNode.Attribute("orientation");
            if (dirAttr != null)
            {
                horiz = (dirAttr.Value == "horizontal");
            }
            meter.Orient = horiz? Orientation.Horizontal : Orientation.Vertical;

            int x = 0; int y = 0;
            meterNode.TryInteger("tickX", out x);
            meterNode.TryInteger("tickY", out y);

            meter.TickOffset = new Point(x, y);

            XElement soundNode = meterNode.Element("Sound");
            if (soundNode != null) meter.Sound = SoundInfo.FromXml(soundNode, basePath);

            XElement bindingNode = meterNode.Element("Binding");
            if (bindingNode != null) meter.Binding = SceneBindingInfo.FromXml(bindingNode);

            return meter;
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Meter");

            writer.WriteAttributeString("x", this.Position.X.ToString());
            writer.WriteAttributeString("y", this.Position.Y.ToString());

            writer.WriteAttributeString("image", TickImage.Relative);

            if (Background != null) writer.WriteAttributeString("background", Background.Relative);

            writer.WriteAttributeString("orientation", (Orient == Orientation.Horizontal ? "horizontal" : "vertical"));

            writer.WriteAttributeString("tickX", TickOffset.X.ToString());
            writer.WriteAttributeString("tickY", TickOffset.Y.ToString());

            if (Sound != null) Sound.Save(writer);

            if (Binding != null) Binding.Save(writer);

            writer.WriteEndElement();
        }
    }
}
