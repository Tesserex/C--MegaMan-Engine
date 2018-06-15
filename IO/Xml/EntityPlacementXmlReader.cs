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

            info.entity = node.TryAttribute("name", node.GetAttribute<string>("entity"));
            
            info.state = node.TryAttribute("state", "Start");

            info.screenX = node.GetAttribute<int>("x");
            info.screenY = node.GetAttribute<int>("y");

            var dirAttr = node.Attribute("direction");
            if (dirAttr != null)
            {
                var dir = Direction.Left;
                Enum.TryParse(dirAttr.Value, true, out dir);
                info.direction = dir;
            }

            var respawnAttr = node.Attribute("respawn");
            if (respawnAttr != null)
            {
                var respawn = RespawnBehavior.Offscreen;
                Enum.TryParse(respawnAttr.Value, true, out respawn);
                info.respawn = respawn;
            }

            return info;
        }
    }
}
