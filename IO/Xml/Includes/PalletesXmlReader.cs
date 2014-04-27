using MegaMan.Common;
using System.Xml.Linq;

namespace MegaMan.IO.Xml.Includes
{
    internal class PalletesXmlReader : IIncludeXmlReader
    {
        public void Load(Project project, XElement xmlNode)
        {
            foreach (var node in xmlNode.Elements("Palette"))
            {
                var palette = PaletteFromXml(node, project.BaseDir);

                project.AddPalette(palette);
            }
        }

        private PaletteInfo PaletteFromXml(XElement node, string baseDir)
        {
            var palette = new PaletteInfo();

            var imagePathRelative = node.RequireAttribute("image").Value;
            palette.ImagePath = FilePath.FromRelative(imagePathRelative, baseDir);
            palette.Name = node.RequireAttribute("name").Value;

            return palette;
        }

        public string NodeName
        {
            get { return "Palettes"; }
        }
    }
}
