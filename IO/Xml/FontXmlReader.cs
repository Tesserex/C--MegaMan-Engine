using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class FontGroupXmlReader : IGameObjectXmlReader
    {
        public void Load(Project project, XElement node)
        {
            foreach (var fontNode in node.Elements("Font"))
            {
                //LoadFont(project, fontNode);
            }
        }
    }

    public class FontXmlReader : IGameObjectXmlReader
    {
        public void Load(Project project, XElement node)
        {
            var info = new FontInfo();

            info.Name = node.RequireAttribute("name").Value;
            info.CharWidth = node.GetAttribute<int>("charwidth");
            info.CaseSensitive = node.GetAttribute<bool>("cased");

            foreach (var lineNode in node.Elements("Line"))
            {
                var x = lineNode.GetAttribute<int>("x");
                var y = lineNode.GetAttribute<int>("y");

                var lineText = lineNode.Value;

                info.AddLine(x, y, lineText);
            }

            info.ImagePath = FilePath.FromRelative(node.RequireAttribute("image").Value, project.BaseDir);

            project.AddFont(info);
        }
    }
}
