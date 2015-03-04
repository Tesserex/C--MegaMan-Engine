using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml
{
    public class MenuXmlReader : HandlerXmlReader, IIncludeXmlReader
    {
        public void Load(Project project, XElement node)
        {
            var menu = new MenuInfo();

            LoadHandlerBase(menu, node, project.BaseDir);

            foreach (var keyNode in node.Elements("State"))
            {
                menu.States.Add(LoadMenuState(keyNode, project.BaseDir));
            }

            project.AddMenu(menu);
        }

        private static MenuStateInfo LoadMenuState(XElement node, string basePath)
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

            info.Commands = LoadCommands(node, basePath);

            return info;
        }

        public string NodeName
        {
            get { return "Menu"; }
        }
    }
}
