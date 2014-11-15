using System;
using System.Collections.Generic;
using System.Xml;

namespace MegaMan.Common
{
    public class ScreenLayerInfo
    {
        public String Name { get; private set; }
        public TileLayer Tiles { get; private set; }
        public Boolean Foreground { get; private set; }
        public Boolean Parallax { get; set; }
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

            writer.WriteAttributeString("x", Tiles.BaseX.ToString());
            writer.WriteAttributeString("y", Tiles.BaseY.ToString());

            if (Foreground)
                writer.WriteAttributeString("foreground", Foreground.ToString());

            if (Parallax)
                writer.WriteAttributeString("parallax", Parallax.ToString());

            foreach (var entity in Entities)
                entity.Save(writer);

            foreach (var keyframe in Keyframes)
                keyframe.Save(writer);

            writer.WriteEndElement();
        }
    }

    public class ScreenLayerKeyframe
    {
        public int Frame { get; set; }
        public ScreenLayerMoveCommand Move { get; set; }
        public bool Reset { get; set; }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Keyframe");

            writer.WriteAttributeString("frame", Frame.ToString());

            if (Move != null)
            {
                writer.WriteStartElement("Move");
                writer.WriteAttributeString("x", Move.X.ToString());
                writer.WriteAttributeString("y", Move.Y.ToString());
                writer.WriteAttributeString("duration", Move.Duration.ToString());
                writer.WriteEndElement();
            }

            if (Reset)
                writer.WriteElementString("Reset", "");

            writer.WriteEndElement();
        }
    }

    public class ScreenLayerMoveCommand
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Duration { get; set; }
    }
}
