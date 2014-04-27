using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml.Includes
{
    internal class FontXmlReader : IIncludeXmlReader
    {
        public void Load(Project project, XElement xmlNode)
        {
            var info = new FontInfo();

            info.Name = xmlNode.RequireAttribute("name").Value;
            info.CharWidth = xmlNode.GetAttribute<int>("charwidth");
            info.CaseSensitive = xmlNode.GetAttribute<bool>("cased");

            foreach (var lineNode in xmlNode.Elements("Line"))
            {
                var x = lineNode.GetAttribute<int>("x");
                var y = lineNode.GetAttribute<int>("y");

                var lineText = lineNode.Value;

                info.AddLine(x, y, lineText);
            }

            info.ImagePath = FilePath.FromRelative(xmlNode.RequireAttribute("image").Value, project.BaseDir);

            project.AddFont(info);
        }

        public string NodeName
        {
            get { return "Font"; }
        }
    }
}
