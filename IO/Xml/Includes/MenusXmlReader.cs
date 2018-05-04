using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;
using MegaMan.IO.Xml.Handlers;

namespace MegaMan.IO.Xml.Includes
{
    internal class MenusXmlReader : IIncludeXmlReader
    {
        private MenuXmlReader _menuReader;

        public MenusXmlReader(MenuXmlReader menuReader)
        {
            _menuReader = menuReader;
        }

        public IIncludedObject Load(Project project, XElement xmlNode, IDataSource dataSource)
        {
            var group = new IncludedObjectGroup();
            foreach (var menuNode in xmlNode.Elements("Menu"))
            {
                group.Add(_menuReader.Load(project, menuNode, dataSource));
            }

            return group;
        }

        public string NodeName
        {
            get { return "Menus"; }
        }
    }
}
