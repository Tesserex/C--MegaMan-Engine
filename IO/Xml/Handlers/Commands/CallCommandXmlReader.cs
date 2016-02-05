using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class CallCommandXmlReader : ICommandXmlReader
    {
        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Call";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneCallCommandInfo();

            info.Name = node.Value;

            return info;
        }
    }
}
