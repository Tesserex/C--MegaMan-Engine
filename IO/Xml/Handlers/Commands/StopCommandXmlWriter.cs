using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class StopCommandXmlWriter : ICommandXmlWriter
    {
        public Type CommandType
        {
            get
            {
                return typeof(SceneStopMusicCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("StopMusic");
            writer.WriteEndElement();
        }
    }
}
