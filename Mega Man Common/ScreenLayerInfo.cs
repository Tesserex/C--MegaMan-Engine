using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace MegaMan.Common
{
    public class ScreenLayerInfo
    {
        public String Name { get; private set; }
        public TileLayer Tiles { get; private set; }
        public Boolean Foreground { get; private set; }
        public List<EntityPlacement> Entities { get; private set; }
        public List<ScreenLayerKeyframe> Keyframes { get; private set; }

        public ScreenLayerInfo(string name, TileLayer tiles, bool foreground, List<EntityPlacement> entities, List<ScreenLayerKeyframe> keyframes)
        {
            this.Name = name;
            this.Tiles = tiles;
            this.Foreground = foreground;
            this.Entities = entities;
            this.Keyframes = keyframes;
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Overlay");

            writer.WriteAttributeString("name", Name);

            if (Foreground)
            {
                writer.WriteAttributeString("foreground", Foreground.ToString());
            }

            foreach (var entity in Entities)
            {
                entity.Save(writer);
            }

            writer.WriteEndElement();
        }
    }

    public class ScreenLayerKeyframe
    {
        public int Frame { get; set; }
        public ScreenLayerMoveCommand Move { get; set; }
        public bool Reset { get; set; }
    }

    public class ScreenLayerMoveCommand
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Duration { get; set; }
    }
}
