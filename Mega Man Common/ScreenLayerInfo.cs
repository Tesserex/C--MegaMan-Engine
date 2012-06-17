using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace MegaMan.Common
{
    public class ScreenLayerInfo
    {
        public String Name { get; private set; }
        public TileLayer Tiles { get; private set; }
        public List<EntityPlacement> Entities { get; private set; }

        public ScreenLayerInfo(string name, TileLayer tiles, List<EntityPlacement> entities)
        {
            this.Name = name;
            this.Tiles = tiles;
            this.Entities = entities;
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Overlay");

            writer.WriteAttributeString("name", Name);

            foreach (var entity in Entities)
            {
                entity.Save(writer);
            }

            writer.WriteEndElement();
        }
    }
}
