using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using MegaMan.Common.Geometry;

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
