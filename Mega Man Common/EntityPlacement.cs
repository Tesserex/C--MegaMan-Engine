using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;

namespace MegaMan.Common
{
    public enum EntityDirection
    {
        Left,
        Right
    }

    public enum RespawnBehavior
    {
        Offscreen,
        Death,
        Stage,
        Never
    }

    public class EntityPlacement
    {
        public string id;
        public string entity;
        public string state;
        public EntityDirection direction;
        public RespawnBehavior respawn;
        public float screenX;
        public float screenY;

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Entity");
            if (id != null)
            {
                writer.WriteAttributeString("id", id);
            }
            writer.WriteAttributeString("name", entity);
            if (state != "Start") writer.WriteAttributeString("state", state);
            writer.WriteAttributeString("x", screenX.ToString());
            writer.WriteAttributeString("y", screenY.ToString());
            writer.WriteAttributeString("direction", direction.ToString());
            writer.WriteAttributeString("respawn", respawn.ToString());
            writer.WriteEndElement();
        }
    }
}
