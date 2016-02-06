using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class RemoveCommandXmlWriter : ICommandXmlWriter
    {
        public Type CommandType
        {
            get
            {
                return typeof(SceneRemoveCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var remove = (SceneRemoveCommandInfo)info;

            writer.WriteStartElement("Remove");
            writer.WriteAttributeString("name", remove.Name);
            writer.WriteEndElement();
        }
    }
}
