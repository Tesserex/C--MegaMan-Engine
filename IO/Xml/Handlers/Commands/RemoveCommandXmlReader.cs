using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class RemoveCommandXmlReader : ICommandXmlReader
    {
        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Remove";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneRemoveCommandInfo();
            info.Name = node.RequireAttribute("name").Value;
            return info;
        }
    }
}
