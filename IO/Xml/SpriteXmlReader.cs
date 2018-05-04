using System;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.IO.DataSources;
using MegaMan.IO;

namespace MegaMan.IO.Xml
{
    internal class SpriteXmlReader
    {
        public Sprite LoadSprite(IDataSource dataSource, XElement element, string basePath)
        {
            var sprite = LoadSprite(element);

            var tileattr = element.RequireAttribute("tilesheet");
            sprite.SheetPath = FilePath.FromRelative(tileattr.Value, basePath);
            sprite.SheetData = dataSource.GetBytesFromFilePath(sprite.SheetPath);

            return sprite;
        }

        public Sprite LoadSprite(XElement element)
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
    }
}
