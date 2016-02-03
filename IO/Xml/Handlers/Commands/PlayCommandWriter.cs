using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class PlayCommandWriter : ICommandXmlWriter
    {
        public Type CommandType
        {
            get
            {
                return typeof(ScenePlayCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var play = (ScenePlayCommandInfo)info;

            writer.WriteStartElement("PlayMusic");
            writer.WriteAttributeString("track", play.Track.ToString());
            writer.WriteEndElement();
        }
    }
}
