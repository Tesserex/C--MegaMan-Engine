using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;

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
