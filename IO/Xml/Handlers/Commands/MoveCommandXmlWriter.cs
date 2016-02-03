using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class MoveCommandXmlWriter : ICommandXmlWriter
    {
        public Type CommandType
        {
            get
            {
                return typeof(SceneMoveCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var move = (SceneMoveCommandInfo)info;

            writer.WriteStartElement("Move");
            if (!string.IsNullOrEmpty(move.Name)) writer.WriteAttributeString("name", move.Name);
            writer.WriteAttributeString("x", move.X.ToString());
            writer.WriteAttributeString("y", move.Y.ToString());
            if (move.Duration > 0) writer.WriteAttributeString("duration", move.Duration.ToString());
            writer.WriteEndElement();
        }
    }
}
