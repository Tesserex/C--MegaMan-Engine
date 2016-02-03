using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class NextCommandXmlWriter : ICommandXmlWriter
    {
        private readonly HandlerTransferXmlWriter _transferWriter;

        public NextCommandXmlWriter(HandlerTransferXmlWriter transferWriter)
        {
            _transferWriter = transferWriter;
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
            _transferWriter.Write(next.NextHandler, writer);
        }
    }
}
