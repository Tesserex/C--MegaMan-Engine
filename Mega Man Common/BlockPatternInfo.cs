using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;
using System.Xml;

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
            info.LeftBoundary = xmlNode.GetInteger("left");
            info.RightBoundary = xmlNode.GetInteger("right");
            info.Length = xmlNode.GetInteger("length");

            info.Blocks = new List<BlockInfo>();
            foreach (XElement blockInfo in xmlNode.Elements("Block"))
            {
                BlockInfo block = new BlockInfo();
                block.pos = new PointF((float)blockInfo.GetDouble("x"), (float)blockInfo.GetDouble("y"));
                block.on = blockInfo.GetInteger("on");
                block.off = blockInfo.GetInteger("off");

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
