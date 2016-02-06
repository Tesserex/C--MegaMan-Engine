using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class FillMoveCommandXmlWriter : ICommandXmlWriter
    {
        public Type CommandType
        {
            get
            {
                return typeof(SceneFillMoveCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var fill = (SceneFillMoveCommandInfo)info;

            writer.WriteStartElement("FillMove");
            if (!string.IsNullOrEmpty(fill.Name)) writer.WriteAttributeString("name", fill.Name);
            writer.WriteAttributeString("x", fill.X.ToString());
            writer.WriteAttributeString("y", fill.Y.ToString());
            writer.WriteAttributeString("width", fill.Width.ToString());
            writer.WriteAttributeString("height", fill.Height.ToString());
            writer.WriteAttributeString("duration", fill.Duration.ToString());
            writer.WriteEndElement();
        }
    }
}
