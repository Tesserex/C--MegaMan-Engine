using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class TextCommandXmlReader : ICommandXmlReader
    {
        private readonly SceneBindingXmlReader bindingReader;

        public TextCommandXmlReader(SceneBindingXmlReader bindingReader)
        {
            this.bindingReader = bindingReader;
        }

        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Text";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneTextCommandInfo();
            info.Content = node.TryAttribute<string>("content");
            info.Name = node.TryAttribute<string>("name");
            info.Speed = node.TryAttribute<int>("speed");
            info.X = node.GetAttribute<int>("x");
            info.Y = node.GetAttribute<int>("y");

            var bindingNode = node.Element("Binding");
            if (bindingNode != null) info.Binding = bindingReader.Load(bindingNode);

            info.Font = node.TryAttribute<string>("font");

            return info;
        }
    }
}
