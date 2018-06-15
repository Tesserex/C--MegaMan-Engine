using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class MoveCommandXmlReader : ICommandXmlReader
    {
        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Move";
                yield return "SpriteMove";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneMoveCommandInfo();
            info.Name = node.RequireAttribute("name").Value;

            info.Duration = node.TryAttribute<int>("duration");

            info.X = node.GetAttribute<int>("x");
            info.Y = node.GetAttribute<int>("y");
            return info;
        }
    }
}
