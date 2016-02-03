using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    public class FillCommandXmlWriter : ICommandXmlWriter
    {
        public Type CommandType
        {
            get
            {
                return typeof(SceneFillCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var fill = (SceneFillCommandInfo)info;

            writer.WriteStartElement("Fill");
            if (!string.IsNullOrEmpty(fill.Name)) writer.WriteAttributeString("name", fill.Name);
            writer.WriteAttributeString("color", fill.Red.ToString() + "," + fill.Green.ToString() + "," + fill.Blue.ToString());
            writer.WriteAttributeString("x", fill.X.ToString());
            writer.WriteAttributeString("y", fill.Y.ToString());
            writer.WriteAttributeString("width", fill.Width.ToString());
            writer.WriteAttributeString("height", fill.Height.ToString());
            writer.WriteAttributeString("layer", fill.Layer.ToString());
            writer.WriteEndElement();
        }
    }
}
