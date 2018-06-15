using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class NextCommandXmlWriter : ICommandXmlWriter
    {
        private readonly HandlerTransferXmlWriter transferWriter;

        public NextCommandXmlWriter(HandlerTransferXmlWriter transferWriter)
        {
            this.transferWriter = transferWriter;
        }

        public Type CommandType
        {
            get
            {
                return typeof(SceneNextCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var next = (SceneNextCommandInfo)info;
            transferWriter.Write(next.NextHandler, writer);
        }
    }
}
