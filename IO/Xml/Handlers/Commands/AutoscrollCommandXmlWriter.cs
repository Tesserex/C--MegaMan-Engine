using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class AutoscrollCommandXmlWriter : ICommandXmlWriter
    {
        public Type CommandType
        {
            get
            {
                return typeof(SceneAutoscrollCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var scroll = (SceneAutoscrollCommandInfo)info;

            writer.WriteStartElement("Autoscroll");

            writer.WriteAttributeString("speed", scroll.Speed.ToString());
            writer.WriteAttributeString("startX", scroll.StartX.ToString());

            writer.WriteEndElement();
        }
    }
}
