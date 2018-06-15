using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.Xml.Handlers.Commands;

namespace MegaMan.IO.Xml.Handlers
{
    internal abstract class HandlerXmlWriter
    {
        protected readonly HandlerCommandXmlWriter CommandWriter;

        public HandlerXmlWriter(HandlerCommandXmlWriter commandWriter)
        {
            CommandWriter = commandWriter;
        }

        protected void WriteBase(HandlerInfo info, XmlWriter writer)
        {
            writer.WriteAttributeString("name", info.Name);

            foreach (var obj in info.Objects.Values)
            {
                if (!objectWriters.ContainsKey(obj.GetType()))
                    throw new Exception("No writer found for handler object: " + obj.GetType().Name);

                var objWriter = objectWriters[obj.GetType()];
                objWriter.Write(obj, writer);
            }
        }

        private static Dictionary<Type, IHandlerObjectXmlWriter> objectWriters;

        static HandlerXmlWriter()
        {
            objectWriters = Extensions.GetImplementersOf<IHandlerObjectXmlWriter>()
                .ToDictionary(x => x.ObjectType);
        }
    }
}
