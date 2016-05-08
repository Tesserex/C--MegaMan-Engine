using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;

namespace MegaMan.IO.Xml.Includes
{
    internal class PalletesXmlReader : IIncludeXmlReader
    {
        public IIncludedObject Load(Project project, XElement xmlNode)
        {
            var group = new IncludedObjectGroup();
            foreach (var node in xmlNode.Elements("Palette"))
            {
                var palette = PaletteFromXml(node, project.BaseDir);
                group.Add(palette);
                project.AddPalette(palette);
            }

            return group;
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
