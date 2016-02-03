using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class MenuOptionCommandXmlWriter : ICommandXmlWriter
    {
        public Type CommandType
        {
            get
            {
                return typeof(MenuOptionCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var option = (MenuOptionCommandInfo)info;

            writer.WriteStartElement("Option");

            if (option.Name != null)
                writer.WriteAttributeString("name", option.Name);

            writer.WriteAttributeString("x", option.X.ToString());
            writer.WriteAttributeString("y", option.Y.ToString());

            if (option.OnEvent != null)
            {
                writer.WriteStartElement("On");
                foreach (var cmd in option.OnEvent)
                {
                    Write(cmd, writer);
                }
                writer.WriteEndElement();
            }

            if (option.OffEvent != null)
            {
                writer.WriteStartElement("Off");
                foreach (var cmd in option.OffEvent)
                {
                    Write(cmd, writer);
                }
                writer.WriteEndElement();
            }

            if (option.SelectEvent != null)
            {
                writer.WriteStartElement("Select");
                foreach (var cmd in option.SelectEvent)
                {
                    Write(cmd, writer);
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }
    }
}
