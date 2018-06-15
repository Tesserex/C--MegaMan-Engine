using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.Common.Geometry;
using MegaMan.IO.DataSources;

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
            var width = element.GetAttribute<int>("width");
            var height = element.GetAttribute<int>("height");

            var sprite = new Sprite(width, height);

            sprite.Name = element.TryAttribute<string>("name");
            sprite.Part = element.TryAttribute<string>("part");
            sprite.PaletteName = element.TryAttribute<string>("palette");

            sprite.Reversed = element.TryAttribute<bool>("reversed");

            var hotspot = element.Element("Hotspot");
            if (hotspot != null)
            {
                var hx = hotspot.GetAttribute<int>("x");
                var hy = hotspot.GetAttribute<int>("y");
                sprite.HotSpot = new Point(hx, hy);
            }
            else
            {
                sprite.HotSpot = new Point(0, 0);
            }

            sprite.Layer = element.TryAttribute<int>("layer");

            var stylenode = element.Element("AnimStyle");
            if (stylenode != null)
            {
                var style = stylenode.Value;
                switch (style)
                {
                    case "Bounce": sprite.AnimStyle = AnimationStyle.Bounce; break;
                    case "PlayOnce": sprite.AnimStyle = AnimationStyle.PlayOnce; break;
                }
            }

            var directionNode = element.Element("AnimDirection");
            if (directionNode != null)
            {
                var direction = directionNode.Value;
                switch (direction)
                {
                    case "Forward": sprite.AnimDirection = AnimationDirection.Forward; break;
                    case "Backward": sprite.AnimDirection = AnimationDirection.Backward; break;
                }
            }

            foreach (var frame in element.Elements("Frame"))
            {
                var duration = frame.TryAttribute<int>("duration");
                var x = frame.GetAttribute<int>("x");
                var y = frame.GetAttribute<int>("y");
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
