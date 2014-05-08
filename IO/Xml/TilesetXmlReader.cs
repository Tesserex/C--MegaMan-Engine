using MegaMan.Common;
using System;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class TilesetXmlReader : GameXmlReader, ITilesetReader
    {
        public Tileset Load(FilePath path)
        {
            var tileset = new Tileset();

            tileset.FilePath = path;

            var doc = XDocument.Load(path.Absolute);
            var reader = doc.Element("Tileset");
            if (reader == null)
                throw new Exception("The specified tileset definition file does not contain a Tileset tag.");

            var sheetPath = FilePath.FromRelative(reader.Attribute("tilesheet").Value, path.BasePath);
            tileset.ChangeSheetPath(sheetPath.Absolute);

            int size;
            if (!int.TryParse(reader.Attribute("tilesize").Value, out size))
                throw new Exception("The tileset definition does not contain a valid tilesize attribute.");
            tileset.TileSize = size;

            var propParent = reader.Element("TileProperties");
            if (propParent != null)
            {
                foreach (XElement propNode in propParent.Elements("Properties"))
                {
                    var prop = new TileProperties(propNode);
                    tileset.AddProperties(prop);
                }
            }

            foreach (XElement tileNode in reader.Elements("Tile"))
            {
                int id = int.Parse(tileNode.Attribute("id").Value);
                string name = tileNode.Attribute("name").Value;

                var spriteNode = tileNode.Element("Sprite");
                if (spriteNode == null)
                    throw new GameXmlException(tileNode, "All Tile tags must contain a Sprite tag.");

                var sprite = LoadSprite(spriteNode);
                var tileSprite = new TileSprite(tileset, sprite);

                Tile tile = new Tile(id, tileSprite);

                string propName = "Default";
                XAttribute propAttr = tileNode.Attribute("properties");
                if (propAttr != null)
                    propName = propAttr.Value;

                tile.Properties = tileset.GetProperties(propName);

                tile.Sprite.Play();
                tileset.Add(tile);
            }

            return tileset;
        }
    }
}
