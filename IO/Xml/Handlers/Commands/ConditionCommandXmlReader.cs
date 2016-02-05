using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class ConditionCommandXmlReader : ICommandXmlReader
    {
        private readonly HandlerCommandXmlReader _commandReader;

        public ConditionCommandXmlReader(HandlerCommandXmlReader commandReader)
        {
            _commandReader = commandReader;
        }

        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Condition";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneConditionCommandInfo();

            info.ConditionExpression = node.RequireAttribute("condition").Value;

            var attr = node.Attribute("entity");
            if (attr != null)
            {
                info.ConditionEntity = attr.Value;
            }

            info.Commands = _commandReader.LoadCommands(node, basePath);

            return info;
        }
    }
}
