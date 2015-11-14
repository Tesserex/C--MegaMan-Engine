using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    internal class PaletteXmlReader : IGameObjectXmlReader
    {
        public void Load(Project project, XElement node)
        {
            var palette = new PaletteInfo();

            var imagePathRelative = node.RequireAttribute("image").Value;
            palette.ImagePath = FilePath.FromRelative(imagePathRelative, project.BaseDir);
            palette.Name = node.RequireAttribute("name").Value;

            project.AddPalette(palette);
        }
    }
}
