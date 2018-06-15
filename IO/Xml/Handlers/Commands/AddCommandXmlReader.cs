using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class AddCommandXmlReader : ICommandXmlReader
    {
        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Sprite";
                yield return "Meter";
                yield return "Add";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneAddCommandInfo();
            var nameAttr = node.Attribute("name");
            if (nameAttr != null) info.Name = nameAttr.Value;
            info.Object = node.RequireAttribute("object").Value;
            info.X = node.GetAttribute<int>("x");
            info.Y = node.GetAttribute<int>("y");
            return info;
        }
    }
}
