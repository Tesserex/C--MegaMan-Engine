using MegaMan.Common;
using System.Xml.Linq;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Includes
{
    internal class SoundsXmlReader : IIncludeXmlReader
    {
        private SoundXmlReader _soundReader;

        public SoundsXmlReader(SoundXmlReader soundReader)
        {
            _soundReader = soundReader;
        }

        public string NodeName
        {
            get { return "Sounds"; }
        }

        public IIncludedObject Load(Project project, XElement xmlNode, IDataSource dataSource)
        {
            var group = new IncludedObjectGroup();
            foreach (var node in xmlNode.Elements("Sound"))
            {
                group.Add(_soundReader.Load(project, node, dataSource));
            }

            return group;
        }
    }
}
