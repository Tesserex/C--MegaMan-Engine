using System.Xml;
using MegaMan.Common;
using MegaMan.IO.Xml.Handlers.Commands;

namespace MegaMan.IO.Xml.Handlers
{
    internal class MenuXmlWriter : HandlerXmlWriter
    {
        public MenuXmlWriter(SceneCommandXmlWriter commandWriter) : base(commandWriter) { }

        public void Write(MenuInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Menu");

            WriteBase(info, writer);

            foreach (var state in info.States)
            {
                WriteState(state, writer);
            }

            writer.WriteEndElement();
        }

        private void WriteState(MenuStateInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("State");
            writer.WriteAttributeString("name", info.Name);

            foreach (var command in info.Commands)
            {
                _commandWriter.Write(command, writer);
            }

            if (info.StartOptionName != null || info.StartOptionVar != null)
            {
                writer.WriteStartElement("SelectOption");
                if (info.StartOptionName != null)
                {
                    writer.WriteAttributeString("name", info.StartOptionName);
                }
                if (info.StartOptionVar != null)
                {
                    writer.WriteAttributeString("var", info.StartOptionVar);
                }
            }

            writer.WriteEndElement();
        }
    }
}
