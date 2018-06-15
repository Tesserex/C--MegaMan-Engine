using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.IO.Xml
{
    public class BlockPatternXmlReader
    {
        public BlockPatternInfo FromXml(XElement xmlNode)
        {
            var info = new BlockPatternInfo();

            info.Entity = xmlNode.RequireAttribute("entity").Value;
            info.LeftBoundary = xmlNode.GetAttribute<int>("left");
            info.RightBoundary = xmlNode.GetAttribute<int>("right");
            info.Length = xmlNode.GetAttribute<int>("length");

            info.Blocks = new List<BlockInfo>();
            foreach (var blockInfo in xmlNode.Elements("Block"))
            {
                var block = new BlockInfo();
                block.Pos = new PointF(blockInfo.GetAttribute<float>("x"), blockInfo.GetAttribute<float>("y"));
                block.On = blockInfo.GetAttribute<int>("on");
                block.Off = blockInfo.GetAttribute<int>("off");

                info.Blocks.Add(block);
            }

            return info;
        }

        public void Save(XmlTextWriter writer, BlockPatternInfo blockPattern)
        {
            writer.WriteStartElement("Blocks");
            writer.WriteAttributeString("left", blockPattern.LeftBoundary.ToString());
            writer.WriteAttributeString("right", blockPattern.RightBoundary.ToString());
            writer.WriteAttributeString("length", blockPattern.Length.ToString());
            writer.WriteAttributeString("entity", blockPattern.Entity);

            foreach (var block in blockPattern.Blocks)
            {
                writer.WriteStartElement("Block");
                writer.WriteAttributeString("x", block.Pos.X.ToString());
                writer.WriteAttributeString("y", block.Pos.Y.ToString());
                writer.WriteAttributeString("on", block.On.ToString());
                writer.WriteAttributeString("off", block.Off.ToString());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
