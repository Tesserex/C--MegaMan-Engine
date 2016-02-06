using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml
{
    internal class SceneBindingXmlWriter
    {
        public void Write(SceneBindingInfo info, XmlWriter writer)
        {
            writer.WriteStartElement("Binding");
            writer.WriteAttributeString("source", info.Source);
            writer.WriteAttributeString("target", info.Target);
            writer.WriteEndElement();
        }
    }
}
