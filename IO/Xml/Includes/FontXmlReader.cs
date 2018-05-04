using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Includes
{
    internal class FontXmlReader : IIncludeXmlReader
    {
        public IIncludedObject Load(Project project, XElement xmlNode, IDataSource dataSource)
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
            info.ImageData = dataSource.GetBytesFromFilePath(info.ImagePath);

            project.AddFont(info);
            return info;
        }

        public string NodeName
        {
            get { return "Font"; }
        }
    }
}
