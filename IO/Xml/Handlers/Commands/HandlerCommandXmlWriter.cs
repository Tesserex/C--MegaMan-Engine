using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    public class HandlerCommandXmlWriter
    {
        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            if (!writers.ContainsKey(info.GetType()))
                throw new Exception("No xml writer found for command type: " + info.GetType().Name);

            var cmdWriter = writers[info.GetType()];
            cmdWriter.Write(info, writer);
        }

        private static Dictionary<Type, ICommandXmlWriter> writers;

        static HandlerCommandXmlWriter()
        {
            writers = Extensions.GetImplementersOf<ICommandXmlWriter>()
                .ToDictionary(x => x.CommandType);
        }
    }
}
