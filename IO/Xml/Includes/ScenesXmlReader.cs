using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;
using MegaMan.IO.Xml.Handlers;

namespace MegaMan.IO.Xml.Includes
{
    internal class ScenesXmlReader : IIncludeXmlReader
    {
        private SceneXmlReader _sceneReader;

        public ScenesXmlReader(SceneXmlReader sceneReader)
        {
            _sceneReader = sceneReader;
        }

        public IIncludedObject Load(Project project, XElement xmlNode, IDataSource dataSource)
        {
            var group = new IncludedObjectGroup();
            foreach (XElement sceneNode in xmlNode.Elements("Scene"))
            {
                group.Add(_sceneReader.Load(project, sceneNode, dataSource));
            }

            return group;
        }

        public string NodeName
        {
            get { return "Scenes"; }
        }
    }
}
