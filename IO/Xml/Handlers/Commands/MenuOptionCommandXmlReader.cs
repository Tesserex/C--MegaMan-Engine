using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class MenuOptionCommandXmlReader : ICommandXmlReader
    {
        private readonly HandlerCommandXmlReader commandReader;

        public MenuOptionCommandXmlReader(HandlerCommandXmlReader commandReader)
        {
            this.commandReader = commandReader;
        }

        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Option";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new MenuOptionCommandInfo();

            var nameAttr = node.Attribute("name");
            if (nameAttr != null)
            {
                info.Name = nameAttr.Value;
            }

            info.X = node.GetAttribute<int>("x");
            info.Y = node.GetAttribute<int>("y");

            var onNode = node.Element("On");
            if (onNode != null)
            {
                info.OnEvent = commandReader.LoadCommands(onNode, basePath);
            }

            var offNode = node.Element("Off");
            if (offNode != null)
            {
                info.OffEvent = commandReader.LoadCommands(offNode, basePath);
            }

            var selectNode = node.Element("Select");
            if (selectNode != null)
            {
                info.SelectEvent = commandReader.LoadCommands(selectNode, basePath);
            }

            return info;
        }
    }
}
