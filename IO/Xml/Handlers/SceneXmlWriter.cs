﻿using System.Xml;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.Xml.Handlers.Commands;

namespace MegaMan.IO.Xml.Handlers
{
    internal class SceneXmlWriter : HandlerXmlWriter
    {
        private readonly HandlerTransferXmlWriter transferWriter;

        public SceneXmlWriter(HandlerCommandXmlWriter commandWriter, HandlerTransferXmlWriter transferWriter)
            : base(commandWriter)
        {
            this.transferWriter = transferWriter;
        }

        public void Write(SceneInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Scene");

            WriteBase(info, writer);

            writer.WriteAttributeString("duration", info.Duration.ToString());
            writer.WriteAttributeString("canskip", info.CanSkip.ToString());

            foreach (var keyframe in info.KeyFrames)
            {
                WriteKeyframe(keyframe, writer);
            }

            if (info.NextHandler != null)
            {
                transferWriter.Write(info.NextHandler, writer);
            }

            writer.WriteEndElement();
        }

        private void WriteKeyframe(KeyFrameInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Keyframe");

            foreach (var command in info.Commands)
            {
                CommandWriter.Write(command, writer);
            }

            writer.WriteEndElement();
        }
    }
}
