using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MegaMan.Common;
using MegaMan.IO.Xml.Handlers.Commands;

namespace MegaMan.IO.Xml.Handlers
{
    internal abstract class HandlerXmlWriter
    {
        protected readonly HandlerCommandXmlWriter _commandWriter;

        public HandlerXmlWriter(HandlerCommandXmlWriter commandWriter)
        {
            _commandWriter = commandWriter;
        }

        protected void WriteBase(HandlerInfo info, XmlWriter writer)
        {
            writer.WriteAttributeString("name", info.Name);

            foreach (var obj in info.Objects.Values)
            {
                if (!_objectWriters.ContainsKey(obj.GetType()))
                    throw new Exception("No writer found for handler object: " + obj.GetType().Name);

                var objWriter = _objectWriters[obj.GetType()];
                objWriter.Write(obj, writer);
            }
        }

        private static Dictionary<Type, IHandlerObjectXmlWriter> _objectWriters;

        static HandlerXmlWriter()
        {
            _objectWriters = Extensions.GetImplementersOf<IHandlerObjectXmlWriter>()
                .ToDictionary(x => x.ObjectType);
        }
    }
}
