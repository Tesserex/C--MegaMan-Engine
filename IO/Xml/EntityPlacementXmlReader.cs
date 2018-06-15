using System;
using System.Xml.Linq;
using MegaMan.Common;

namespace MegaMan.IO.Xml
{
    internal class EntityPlacementXmlReader
    {
        public EntityPlacement Load(XElement node)
        {
            var info = new EntityPlacement();

            info.Id = node.TryAttribute("id", Guid.NewGuid().ToString());

            info.Entity = node.TryAttribute("name", node.GetAttribute<string>("entity"));
            
            info.State = node.TryAttribute("state", "Start");

            info.ScreenX = node.GetAttribute<int>("x");
            info.ScreenY = node.GetAttribute<int>("y");

            var dirAttr = node.Attribute("direction");
            if (dirAttr != null)
            {
                var dir = Direction.Left;
                Enum.TryParse(dirAttr.Value, true, out dir);
                info.Direction = dir;
            }

            var respawnAttr = node.Attribute("respawn");
            if (respawnAttr != null)
            {
                var respawn = RespawnBehavior.Offscreen;
                Enum.TryParse(respawnAttr.Value, true, out respawn);
                info.Respawn = respawn;
            }

            return info;
        }
    }
}
