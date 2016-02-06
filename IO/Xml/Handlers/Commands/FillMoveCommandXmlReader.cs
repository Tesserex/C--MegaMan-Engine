using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class FillMoveCommandXmlReader : ICommandXmlReader
    {
        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "FillMove";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneFillMoveCommandInfo();
            info.Name = node.RequireAttribute("name").Value;
            info.Duration = node.GetAttribute<int>("duration");
            info.X = node.GetAttribute<int>("x");
            info.Y = node.GetAttribute<int>("y");
            info.Width = node.GetAttribute<int>("width");
            info.Height = node.GetAttribute<int>("height");
            return info;
        }
    }
}
