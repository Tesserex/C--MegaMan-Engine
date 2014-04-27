using MegaMan.Common;
using System.Xml.Linq;

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

        public void Load(Project project, XElement xmlNode)
        {
            foreach (var node in xmlNode.Elements("Sound"))
            {
                _soundReader.Load(project, node);
            }
        }
    }
}
