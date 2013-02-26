using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using MegaMan.Common.Geometry;

namespace MegaMan.Common
{
    public class ScreenInfoFactory
    {
        public static ScreenInfo FromXml(XElement node, FilePath stagePath, Tileset tileset)
        {
            string id = node.RequireAttribute("id").Value;

            var screen = new ScreenInfo(id, tileset);

            screen.Layers.Add(LoadScreenLayer(node, stagePath.Absolute, id, tileset, 0, 0, false));

            foreach (var overlay in node.Elements("Overlay"))
            {
                var name = overlay.RequireAttribute("name").Value;
                var x = overlay.GetInteger("x");
                var y = overlay.GetInteger("y");
                bool foreground = false;
                overlay.TryBool("foreground", out foreground);

                screen.Layers.Add(LoadScreenLayer(overlay, stagePath.Absolute, name, tileset, x, y, foreground));
            }

            foreach (XElement teleport in node.Elements("Teleport"))
            {
                TeleportInfo info;
                int from_x, from_y, to_x, to_y;
                teleport.TryInteger("from_x", out from_x);
                teleport.TryInteger("from_y", out from_y);
                teleport.TryInteger("to_x", out to_x);
                teleport.TryInteger("to_y", out to_y);
                info.From = new Point(from_x, from_y);
                info.To = new Point(to_x, to_y);
                info.TargetScreen = teleport.Attribute("to_screen").Value;

                screen.Teleports.Add(info);
            }

            var blocks = new List<BlockPatternInfo>();

            foreach (XElement blockNode in node.Elements("Blocks"))
            {
                BlockPatternInfo pattern = BlockPatternInfo.FromXml(blockNode);
                screen.BlockPatterns.Add(pattern);
            }

            screen.Commands = SceneCommandInfo.Load(node, stagePath.BasePath);

            return screen;
        }

        private static ScreenLayerInfo LoadScreenLayer(XElement node, string stagePath, string name, Tileset tileset, int tileStartX, int tileStartY, bool foreground)
        {
            var tileFilePath = Path.Combine(stagePath, name + ".scn");

            var tileArray = LoadTiles(tileFilePath);
            var tileLayer = new TileLayer(tileArray, tileset, tileStartX, tileStartY);

            var entities = new List<EntityPlacement>();

            foreach (XElement entity in node.Elements("Entity"))
            {
                EntityPlacement info = EntityPlacement.FromXml(entity);
                entities.Add(info);
            }

            var keyframes = new List<ScreenLayerKeyframe>();
            foreach (var keyframeNode in node.Elements("Keyframe"))
            {
                var frame = ScreenLayerKeyframe.FromXml(keyframeNode);
                keyframes.Add(frame);
            }

            return new ScreenLayerInfo(name, tileLayer, foreground, entities, keyframes);
        }

        private static int[,] LoadTiles(string filepath)
        {
            string[] lines = File.ReadAllLines(filepath);
            string[] firstline = lines[0].Split(' ');
            int width = int.Parse(firstline[0]);
            int height = int.Parse(firstline[1]);

            int[,] tiles = new int[width, height];
            for (int y = 0; y < height; y++)
            {
                string[] line = lines[y + 1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < width; x++)
                {
                    int id = int.Parse(line[x]);
                    tiles[x,y] = id;
                }
            }

            return tiles;
        }
    }
}
