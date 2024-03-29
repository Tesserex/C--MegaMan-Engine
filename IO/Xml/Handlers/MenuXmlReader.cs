﻿using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.IncludedObjects;
using MegaMan.IO.DataSources;
using MegaMan.IO.Xml.Handlers.Commands;

namespace MegaMan.IO.Xml.Handlers
{
    internal class MenuXmlReader : HandlerXmlReader, IIncludeXmlReader
    {
        private readonly HandlerCommandXmlReader commandReader;

        public MenuXmlReader(HandlerCommandXmlReader commandReader)
        {
            this.commandReader = commandReader;
        }

        public IIncludedObject Load(Project project, XElement node, IDataSource dataSource)
        {
            var menu = new MenuInfo();

            LoadBase(menu, node, project.BaseDir, dataSource);

            foreach (var keyNode in node.Elements("State"))
            {
                menu.States.Add(LoadMenuState(keyNode, project.BaseDir));
            }

            project.AddMenu(menu);
            return menu;
        }

        private MenuStateInfo LoadMenuState(XElement node, string basePath)
        {
            var info = new MenuStateInfo();

            info.Name = node.RequireAttribute("name").Value;

            info.Fade = node.TryAttribute<bool>("fade");

            var startNode = node.Element("SelectOption");
            if (startNode != null)
            {
                var startNameAttr = startNode.Attribute("name");
                var startVarAttr = startNode.Attribute("var");

                if (startNameAttr != null)
                {
                    info.StartOptionName = startNameAttr.Value;
                }

                if (startVarAttr != null)
                {
                    info.StartOptionVar = startVarAttr.Value;
                }
            }

            info.Commands = commandReader.LoadCommands(node, basePath);

            return info;
        }

        public string NodeName
        {
            get { return "Menu"; }
        }
    }
}
