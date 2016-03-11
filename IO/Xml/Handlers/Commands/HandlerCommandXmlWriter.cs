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
            if (!_writers.ContainsKey(info.GetType()))
                throw new Exception("No xml writer found for command type: " + info.GetType().Name);

            var cmdWriter = _writers[info.GetType()];
            cmdWriter.Write(info, writer);
        }

        private static Dictionary<Type, ICommandXmlWriter> _writers;

        static HandlerCommandXmlWriter()
        {
            _writers = Extensions.GetImplementersOf<ICommandXmlWriter>()
                .ToDictionary(x => x.CommandType);
        }
    }
}
