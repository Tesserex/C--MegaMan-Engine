using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml.Includes
{
    internal class SoundsXmlReader : IIncludeXmlReader
    {
        private SoundXmlReader soundReader;

        public SoundsXmlReader(SoundXmlReader soundReader)
        {
            this.soundReader = soundReader;
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
                group.Add(soundReader.Load(project, node, dataSource));
            }

            return group;
        }
    }
}
