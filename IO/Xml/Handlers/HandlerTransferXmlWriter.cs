using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers
{
    internal class HandlerTransferXmlWriter
    {
        public void Write(HandlerTransfer info, XmlWriter writer)
        {
            writer.WriteStartElement("Next");

            if (info.Mode != HandlerMode.Next)
            {
                writer.WriteAttributeString("mode", info.Mode.ToString());
            }

            if (info.Mode == HandlerMode.Push)
            {
                writer.WriteAttributeString("pause", info.Pause.ToString());
            }

            if (info.Mode != HandlerMode.Pop)
            {
                writer.WriteAttributeString("type", Enum.GetName(typeof(HandlerType), info.Type));
                writer.WriteAttributeString("name", info.Name);
            }

            writer.WriteAttributeString("fade", info.Fade.ToString());

            writer.WriteEndElement();
        }
    }
}
