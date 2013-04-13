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
        public List<SceneCommandInfo> Commands { get; set; }
        public string StartOptionName { get; set; }
        public string StartOptionVar { get; set; }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("State");
            writer.WriteAttributeString("name", Name);

            foreach (var command in Commands)
            {
                command.Save(writer);
            }

            if (StartOptionName != null || StartOptionVar != null)
            {
                writer.WriteStartElement("SelectOption");
                if (StartOptionName != null)
                {
                    writer.WriteAttributeString("name", StartOptionName);
                }
                if (StartOptionVar != null)
                {
                    writer.WriteAttributeString("var", StartOptionVar);
                }
            }

            writer.WriteEndElement();
        }
    }

    
}
