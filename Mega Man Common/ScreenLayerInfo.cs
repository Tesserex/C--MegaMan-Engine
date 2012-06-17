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
        public List<EntityPlacement> Entities { get; private set; }
        public List<ScreenLayerKeyframe> Keyframes { get; private set; }

        public ScreenLayerInfo(string name, TileLayer tiles, List<EntityPlacement> entities, List<ScreenLayerKeyframe> keyframes)
        {
            this.Name = name;
            this.Tiles = tiles;
            this.Entities = entities;
            this.Keyframes = keyframes;
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

    public class ScreenLayerKeyframe
    {
        public int Frame { get; private set; }
        public ScreenLayerMoveCommand Move { get; private set; }
        public bool Reset { get; private set; }

        public static ScreenLayerKeyframe FromXml(XElement node)
        {
            var frameNumber = node.GetInteger("frame");

            var keyframe = new ScreenLayerKeyframe();
            keyframe.Frame = frameNumber;

            var moveNode = node.Element("Move");
            if (moveNode != null)
            {
                keyframe.Move = ScreenLayerMoveCommand.FromXml(moveNode);
            }

            keyframe.Reset = node.Elements("Reset").Any();

            return keyframe;
        }
    }

    public class ScreenLayerMoveCommand
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Duration { get; private set; }

        public static ScreenLayerMoveCommand FromXml(XElement node)
        {
            var info = new ScreenLayerMoveCommand();
            info.X = node.GetInteger("x");
            info.Y = node.GetInteger("y");
            info.Duration = node.GetInteger("duration");

            return info;
        }
    }
}
