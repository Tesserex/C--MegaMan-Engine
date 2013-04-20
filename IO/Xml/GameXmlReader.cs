using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class GameXmlReader
    {
        public static EntityPlacement LoadEntityPlacement(XElement entity)
        {
            EntityPlacement info = new EntityPlacement();

            var idAttr = entity.Attribute("id");
            if (idAttr != null)
            {
                info.id = idAttr.Value;
            }

            var nameAttr = entity.RequireAttribute("entity");
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

                    case "stageselect":
                        transfer.Type = HandlerType.StageSelect;
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

        public static void LoadPalettes<T>(XElement parentNode, string basePath) where T : Palette, new()
        {
            foreach (var node in parentNode.Elements("Palette"))
            {
                var palette = PaletteFromXml<T>(node, basePath);

                Palette.Add(palette.Name, palette);
            }
        }

        private static Palette PaletteFromXml<T>(XElement node, string basePath) where T : Palette, new()
        {
            var imagePathRelative = node.RequireAttribute("image").Value;
            var imagePath = FilePath.FromRelative(imagePathRelative, basePath);
            var name = node.RequireAttribute("name").Value;

            var palette = new T();
            palette.Initialize(name, imagePath);
            return palette;
        }
    }
}
