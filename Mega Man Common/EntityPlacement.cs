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
        public string entity;
        public string state;
        public EntityDirection direction;
        public RespawnBehavior respawn;
        public float screenX;
        public float screenY;
        public bool boss;

        public static EntityPlacement FromXml(XElement entity)
        {
            EntityPlacement info = new EntityPlacement();

            var nameAttr = entity.RequireAttribute("name");
            info.entity = nameAttr.Value;

            string state = "Start";
            XAttribute stateAttr = entity.Attribute("state");
            if (stateAttr != null) state = stateAttr.Value;
            info.state = state;

            info.screenX = entity.GetInteger("x");
            info.screenY = entity.GetInteger("y");

            var dirAttr = entity.Attribute("direction");
            if (dirAttr != null)
            {
                EntityDirection dir = EntityDirection.Left;
                Enum.TryParse<EntityDirection>(dirAttr.Value, true, out dir);
                info.direction = dir;
            }

            var respawnAttr = entity.Attribute("respawn");
            if (respawnAttr != null)
            {
                RespawnBehavior respawn = RespawnBehavior.Offscreen;
                Enum.TryParse<RespawnBehavior>(respawnAttr.Value, true, out respawn);
                info.respawn = respawn;
            }

            return info;
        }

        public void Save(XmlTextWriter writer)
        {
            writer.WriteStartElement("Entity");
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
