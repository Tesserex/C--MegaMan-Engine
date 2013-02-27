using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using MegaMan.Common.Geometry;

namespace MegaMan.Common
{
    public class BlockPatternInfo
    {
        public class BlockInfo
        {
            public PointF pos;
            public int on;
            public int off;
        }

        public string Entity { get; private set; }
        public int Length { get; private set; }
        public List<BlockInfo> Blocks { get; private set; }
        public int LeftBoundary { get; private set; }
        public int RightBoundary { get; private set; }

        public static BlockPatternInfo FromXml(XElement xmlNode)
        {
            var info = new BlockPatternInfo();

            info.Entity = xmlNode.RequireAttribute("entity").Value;
            info.LeftBoundary = xmlNode.GetAttribute<int>("left");
            info.RightBoundary = xmlNode.GetAttribute<int>("right");
            info.Length = xmlNode.GetAttribute<int>("length");

            info.Blocks = new List<BlockInfo>();
            foreach (XElement blockInfo in xmlNode.Elements("Block"))
            {
                BlockInfo block = new BlockInfo();
                block.pos = new PointF(blockInfo.GetAttribute<float>("x"), blockInfo.GetAttribute<float>("y"));
                block.on = blockInfo.GetAttribute<int>("on");
                block.off = blockInfo.GetAttribute<int>("off");

                info.Blocks.Add(block);
            }

            return info;
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Blocks");
            writer.WriteAttributeString("left", LeftBoundary.ToString());
            writer.WriteAttributeString("right", RightBoundary.ToString());
            writer.WriteAttributeString("length", Length.ToString());
            writer.WriteAttributeString("entity", Entity);

            foreach (BlockPatternInfo.BlockInfo block in Blocks)
            {
                writer.WriteStartElement("Block");
                writer.WriteAttributeString("x", block.pos.X.ToString());
                writer.WriteAttributeString("y", block.pos.Y.ToString());
                writer.WriteAttributeString("on", block.on.ToString());
                writer.WriteAttributeString("off", block.off.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
