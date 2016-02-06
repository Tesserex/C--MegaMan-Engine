using System.Xml;
using MegaMan.Common;

namespace MegaMan.IO.Xml
{
    public class EntityPlacementXmlWriter
    {
        public void Write(EntityPlacement info, XmlWriter writer)
        {
            writer.WriteStartElement("Entity");
            if (info.Id != null)
            {
                writer.WriteAttributeString("id", info.Id);
            }
            writer.WriteAttributeString("entity", info.entity);
            if (info.state != "Start") writer.WriteAttributeString("state", info.state);
            writer.WriteAttributeString("x", info.screenX.ToString());
            writer.WriteAttributeString("y", info.screenY.ToString());
            writer.WriteAttributeString("direction", info.direction.ToString());
            writer.WriteAttributeString("respawn", info.respawn.ToString());
            writer.WriteEndElement();
        }
    }
}
