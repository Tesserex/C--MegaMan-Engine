using System;
using System.Xml.Linq;
using MegaMan.Common;
using MegaMan.IO.DataSources;

namespace MegaMan.IO.Xml
{
    public class TilesetXmlReader : ITilesetReader
    {
        private IDataSource _dataSource;
        private readonly SpriteXmlReader _spriteReader;

        public TilesetXmlReader()
        {
            _spriteReader = new SpriteXmlReader();
        }

        public void Init(IDataSource dataSource)
        {
            this._dataSource = dataSource;
        }

        public Tileset Load(FilePath path)
        {
            var tileset = new Tileset();

            tileset.FilePath = path;

            var stream = _dataSource.GetData(path);
            var doc = XDocument.Load(stream);
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
                    var prop = LoadProperties(propNode);
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

                var sprite = _spriteReader.LoadSprite(spriteNode);
                var tileSprite = new TileSprite(tileset, sprite);

                Tile tile = new Tile(id, tileSprite);

                string propName = "Default";
                XAttribute propAttr = tileNode.Attribute("properties");
                if (propAttr != null)
                    propName = propAttr.Value;

                tile.Properties = tileset.GetProperties(propName);
                tileset.Add(tile);
            }

            stream.Close();

            return tileset;
        }

        public TileProperties LoadProperties(XElement node)
        {
            var properties = new TileProperties();
            properties.Name = "Default";
            foreach (XAttribute attr in node.Attributes())
            {
                bool b;
                float f;
                switch (attr.Name.LocalName.ToLower())
                {
                    case "name":
                        properties.Name = attr.Value;
                        break;

                    case "blocking":
                        if (!bool.TryParse(attr.Value, out b)) throw new Exception("Tile property blocking attribute was not a valid bool.");
                        properties.Blocking = b;
                        break;

                    case "climbable":
                        if (!bool.TryParse(attr.Value, out b)) throw new Exception("Tile property climbable attribute was not a valid bool.");
                        properties.Climbable = b;
                        break;

                    case "lethal":
                        if (!bool.TryParse(attr.Value, out b)) throw new Exception("Tile property lethal attribute was not a valid bool.");
                        properties.Lethal = b;
                        break;

                    case "pushx":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property pushX attribute was not a valid number.");
                        properties.PushX = f;
                        break;

                    case "pushy":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property pushY attribute was not a valid number.");
                        properties.PushY = f;
                        break;

                    case "resistx":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property resistX attribute was not a valid number.");
                        properties.ResistX = f;
                        break;

                    case "resisty":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property resistY attribute was not a valid number.");
                        properties.ResistY = f;
                        break;

                    case "dragx":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property dragX attribute was not a valid number.");
                        properties.DragX = f;
                        break;

                    case "dragy":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property dragY attribute was not a valid number.");
                        properties.DragY = f;
                        break;

                    case "sinking":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property sinking attribute was not a valid number.");
                        properties.Sinking = f;
                        break;

                    case "gravitymult":
                        if (!attr.Value.TryParse(out f)) throw new Exception("Tile property gravitymult attribute was not a valid number.");
                        properties.GravityMult = f;
                        break;

                    case "onenter":
                        properties.OnEnter = attr.Value;
                        break;

                    case "onleave":
                        properties.OnLeave = attr.Value;
                        break;

                    case "onover":
                        properties.OnOver = attr.Value;
                        break;
                }
            }

            return properties;
        }
    }
}
