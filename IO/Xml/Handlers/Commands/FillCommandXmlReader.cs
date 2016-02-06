using System;
using System.Collections.Generic;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class FillCommandXmlReader : ICommandXmlReader
    {
        public IEnumerable<string> NodeName
        {
            get
            {
                yield return "Fill";
            }
        }

        public SceneCommandInfo Load(XElement node, string basePath)
        {
            var info = new SceneFillCommandInfo();
            var nameAttr = node.Attribute("name");
            if (nameAttr != null) info.Name = nameAttr.Value;
            var colorAttr = node.RequireAttribute("color");
            var color = colorAttr.Value;
            var split = color.Split(',');
            info.Red = byte.Parse(split[0]);
            info.Green = byte.Parse(split[1]);
            info.Blue = byte.Parse(split[2]);
            info.X = node.GetAttribute<int>("x");
            info.Y = node.GetAttribute<int>("y");
            info.Width = node.GetAttribute<int>("width");
            info.Height = node.GetAttribute<int>("height");
            info.Layer = node.TryAttribute<int>("layer");
            return info;
        }
    }
}
