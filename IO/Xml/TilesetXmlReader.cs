using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace MegaMan.IO.Xml
{
    public class TilesetXmlReader : GameXmlReader
    {
        public Tileset LoadTilesetFromXml(FilePath path)
        {
            var tileset = new Tileset();

            tileset.FilePath = path;

            var doc = XDocument.Load(path.Absolute);
            var reader = doc.Element("Tileset");
            if (reader == null)
                throw new Exception("The specified tileset definition file does not contain a Tileset tag.");

            tileset.SheetPath = FilePath.FromRelative(reader.Attribute("tilesheet").Value, path.BasePath);

            int size;
            if (!int.TryParse(reader.Attribute("tilesize").Value, out size)) 
                throw new Exception("The tileset definition does not contain a valid tilesize attribute.");
            tileset.TileSize = size;

            var propParent = reader.Element("TileProperties");
            if (propParent != null) {
                foreach (XElement propNode in propParent.Elements("Properties")) {
                    var prop = new TileProperties(propNode);
                    tileset.AddProperties(prop);
                }
            }

            foreach (XElement tileNode in reader.Elements("Tile")) 
            {
                int id = int.Parse(tileNode.Attribute("id").Value);
                string name = tileNode.Attribute("name").Value;
                var sprite = Sprite.Empty;

                var spriteNode = tileNode.Element("Sprite");
                if (spriteNode != null)
                {
                    sprite = LoadSprite(spriteNode);
                }

                Tile tile = new Tile(id, sprite);

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
