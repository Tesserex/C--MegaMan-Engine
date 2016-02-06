using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class CallCommandXmlWriter : ICommandXmlWriter
    {
        public Type CommandType
        {
            get
            {
                return typeof(SceneCallCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var call = (SceneCallCommandInfo)info;
            writer.WriteStartElement("Call");
            writer.WriteValue(call.Name);
            writer.WriteEndElement();
        }
    }
}
