using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public class MenuInfo : HandlerInfo
    {
        public List<MenuStateInfo> States { get; private set; }

        public MenuInfo()
        {
            States = new List<MenuStateInfo>();
        }

        public static MenuInfo FromXml(XElement node, string basePath)
        {
            var info = new MenuInfo();

            info.Load(node, basePath);

            return info;
        }

        protected override void Load(XElement node, string basePath)
        {
            base.Load(node, basePath);

            foreach (var keyNode in node.Elements("State"))
            {
                this.States.Add(MenuStateInfo.FromXml(keyNode, basePath));
            }
        }

        public override void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Menu");

            base.Save(writer);

            foreach (var state in States)
            {
                state.Save(writer);
            }

            writer.WriteEndElement();
        }
    }

    public class MenuStateInfo
    {
        public string Name { get; set; }
        public bool Fade { get; set; }
        public List<SceneCommandInfo> Commands { get; private set; }

        public static MenuStateInfo FromXml(XElement node, string basePath)
        {
            var info = new MenuStateInfo();

            info.Name = node.RequireAttribute("name").Value;

            bool fade = false;
            node.TryBool("fade", out fade);
            info.Fade = fade;

            info.Commands = SceneCommandInfo.Load(node, basePath);

            return info;
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("State");
            writer.WriteAttributeString("name", Name);

            foreach (var command in Commands)
            {
                command.Save(writer);
            }

            writer.WriteEndElement();
        }
    }

    
}
