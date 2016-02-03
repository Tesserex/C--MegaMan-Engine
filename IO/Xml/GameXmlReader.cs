using System;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.IO.Xml
{
    public class GameXmlReader
    {
        internal static EntityPlacement LoadEntityPlacement(XElement entity)
        {
            EntityPlacement info = new EntityPlacement();

            info.Id = entity.TryAttribute<string>("id", Guid.NewGuid().ToString());

            var nameAttr = entity.Attribute("name");
            if (nameAttr == null)
                nameAttr = entity.RequireAttribute("entity");

            info.entity = nameAttr.Value;

            string state = "Start";
            XAttribute stateAttr = entity.Attribute("state");
            if (stateAttr != null) state = stateAttr.Value;
            info.state = state;

            info.screenX = entity.GetAttribute<int>("x");
            info.screenY = entity.GetAttribute<int>("y");

            var dirAttr = entity.Attribute("direction");
            if (dirAttr != null)
            {
                Direction dir = Direction.Left;
                Enum.TryParse<Direction>(dirAttr.Value, true, out dir);
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

        public static HandlerTransfer LoadHandlerTransfer(XElement node)
        {
            HandlerTransfer transfer = new HandlerTransfer();

            var modeAttr = node.Attribute("mode");
            var mode = HandlerMode.Next;
            if (modeAttr != null)
            {
                Enum.TryParse<HandlerMode>(modeAttr.Value, true, out mode);
            }

            transfer.Mode = mode;

            if (mode == HandlerMode.Push)
            {
                transfer.Pause = node.TryAttribute<bool>("pause");
            }

            if (mode != HandlerMode.Pop)
            {
                switch (node.RequireAttribute("type").Value.ToLower())
                {
                    case "stage":
                        transfer.Type = HandlerType.Stage;
                        break;

                    case "scene":
                        transfer.Type = HandlerType.Scene;
                        break;

                    case "menu":
                        transfer.Type = HandlerType.Menu;
                        break;
                }

                transfer.Name = node.RequireAttribute("name").Value;
            }

            transfer.Fade = node.TryAttribute<bool>("fade");

            return transfer;
        }
    }
}
