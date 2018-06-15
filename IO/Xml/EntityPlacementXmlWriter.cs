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
            writer.WriteAttributeString("entity", info.Entity);
            if (info.State != "Start") writer.WriteAttributeString("state", info.State);
            writer.WriteAttributeString("x", info.ScreenX.ToString());
            writer.WriteAttributeString("y", info.ScreenY.ToString());
            writer.WriteAttributeString("direction", info.Direction.ToString());
            writer.WriteAttributeString("respawn", info.Respawn.ToString());
            writer.WriteEndElement();
        }
    }
}
