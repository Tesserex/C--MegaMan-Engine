using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class AddCommandXmlWriter : ICommandXmlWriter
    {
        public Type CommandType
        {
            get
            {
                return typeof(SceneAddCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var add = (SceneAddCommandInfo)info;

            writer.WriteStartElement("Add");
            if (!string.IsNullOrEmpty(add.Name)) writer.WriteAttributeString("name", add.Name);
            writer.WriteAttributeString("object", add.Object);
            writer.WriteAttributeString("x", add.X.ToString());
            writer.WriteAttributeString("y", add.Y.ToString());
            writer.WriteEndElement();
        }
    }
}
