using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml.Includes
{
    internal class MenusXmlReader : IIncludeXmlReader
    {
        private MenuXmlReader _menuReader;

        public MenusXmlReader(MenuXmlReader menuReader)
        {
            _menuReader = menuReader;
        }

        public void Load(Project project, XElement xmlNode)
        {
            foreach (var menuNode in xmlNode.Elements("Menu"))
            {
                AddMenu(menuNode, project);
            }
        }

        private void AddMenu(XElement node, Project project)
        {
            _menuReader.Load(project, node);
        }

        public string NodeName
        {
            get { return "Menus"; }
        }
    }
}
