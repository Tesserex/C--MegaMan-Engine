using System;
using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml.Handlers.Commands
{
    internal class TextCommandXmlWriter : ICommandXmlWriter
    {
        private readonly SceneBindingXmlWriter bindingWriter;

        public TextCommandXmlWriter(SceneBindingXmlWriter bindingWriter)
        {
            this.bindingWriter = bindingWriter;
        }

        public Type CommandType
        {
            get
            {
                return typeof(SceneTextCommandInfo);
            }
        }

        public void Write(SceneCommandInfo info, XmlWriter writer)
        {
            var text = (SceneTextCommandInfo)info;

            writer.WriteStartElement("Text");
            if (!string.IsNullOrEmpty("Font")) writer.WriteAttributeString("font", text.Font);
            if (!string.IsNullOrEmpty(text.Name)) writer.WriteAttributeString("name", text.Name);
            writer.WriteAttributeString("content", text.Content);
            if (text.Speed != null) writer.WriteAttributeString("speed", text.Speed.Value.ToString());
            writer.WriteAttributeString("x", text.X.ToString());
            writer.WriteAttributeString("y", text.Y.ToString());

            if (text.Binding != null)
                bindingWriter.Write(text.Binding, writer);

            writer.WriteEndElement();
        }
    }
}
