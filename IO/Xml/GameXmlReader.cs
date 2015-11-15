using System;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.IO.Xml
{
    public class GameXmlReader
    {
        internal static Sprite LoadSprite(XElement element, string basePath)
        {
            var sprite = LoadSprite(element);

            var tileattr = element.RequireAttribute("tilesheet");
            sprite.SheetPath = FilePath.FromRelative(tileattr.Value, basePath);

            return sprite;
        }

        internal static Sprite LoadSprite(XElement element)
        {
            int width = element.GetAttribute<int>("width");
            int height = element.GetAttribute<int>("height");

            Sprite sprite = new Sprite(width, height);

            sprite.Name = element.TryAttribute<string>("name");
            sprite.Part = element.TryAttribute<string>("part");
            sprite.PaletteName = element.TryAttribute<string>("palette");

            sprite.Reversed = element.TryAttribute<bool>("reversed");

            XElement hotspot = element.Element("Hotspot");
            if (hotspot != null)
            {
                int hx = hotspot.GetAttribute<int>("x");
                int hy = hotspot.GetAttribute<int>("y");
                sprite.HotSpot = new Point(hx, hy);
            }
            else
            {
                sprite.HotSpot = new Point(0, 0);
            }

            sprite.Layer = element.TryAttribute<int>("layer");

            XElement stylenode = element.Element("AnimStyle");
            if (stylenode != null)
            {
                string style = stylenode.Value;
                switch (style)
                {
                    case "Bounce": sprite.AnimStyle = AnimationStyle.Bounce; break;
                    case "PlayOnce": sprite.AnimStyle = AnimationStyle.PlayOnce; break;
                }
            }

            XElement directionNode = element.Element("AnimDirection");
            if (directionNode != null)
            {
                string direction = directionNode.Value;
                switch (direction)
                {
                    case "Forward": sprite.AnimDirection = AnimationDirection.Forward; break;
                    case "Backward": sprite.AnimDirection = AnimationDirection.Backward; break;
                }
            }

            foreach (XElement frame in element.Elements("Frame"))
            {
                int duration = frame.TryAttribute<int>("duration");
                int x = frame.GetAttribute<int>("x");
                int y = frame.GetAttribute<int>("y");
                sprite.AddFrame(x, y, duration);
            }

            if (sprite.Count == 0)
            {
                sprite.AddFrame(0, 0, 0);
            }

            return sprite;
        }

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
