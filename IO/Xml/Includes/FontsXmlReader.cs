using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Includes
{
    internal class FontsXmlReader : IIncludeXmlReader
    {
        private FontXmlReader fontReader;

        public FontsXmlReader(FontXmlReader fontReader)
        {
            this.fontReader = fontReader;
        }

        public IIncludedObject Load(Project project, XElement xmlNode, IDataSource dataSource)
        {
            var group = new IncludedObjectGroup();
            foreach (var fontNode in xmlNode.Elements("Font"))
            {
                group.Add(fontReader.Load(project, fontNode, dataSource));
            }

            return group;
        }

        public string NodeName
        {
            get { return "Fonts"; }
        }
    }
}
