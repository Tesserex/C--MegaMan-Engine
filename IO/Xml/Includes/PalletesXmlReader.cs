using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Includes
{
    internal class PalletesXmlReader : IIncludeXmlReader
    {
        public IIncludedObject Load(Project project, XElement xmlNode, IDataSource dataSource)
        {
            var group = new IncludedObjectGroup();
            foreach (var node in xmlNode.Elements("Palette"))
            {
                var palette = PaletteFromXml(node, project.BaseDir, dataSource);
                group.Add(palette);
                project.AddPalette(palette);
            }

            return group;
        }

        private PaletteInfo PaletteFromXml(XElement node, string baseDir, IDataSource dataSource)
        {
            var palette = new PaletteInfo();

            var imagePathRelative = node.RequireAttribute("image").Value;
            palette.ImagePath = FilePath.FromRelative(imagePathRelative, baseDir);
            palette.Name = node.RequireAttribute("name").Value;
            palette.ImageData = dataSource.GetBytesFromFilePath(palette.ImagePath);

            return palette;
        }

        public string NodeName
        {
            get { return "Palettes"; }
        }
    }
}
