using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace MegaMan.Common
{
    public class Tileset : List<Tile>
    {
        private Dictionary<string, TileProperties> properties;
        public IEnumerable<TileProperties> Properties { get { return properties.Values; } }

        public FilePath SheetPath
        {
            get;
            private set;
        }

        public FilePath FilePath
        {
            get;
            private set;
        }

        public int TileSize { get; private set; }

        /// <summary>
        /// Construct a Tileset by specifying an absolute path to a tileset XML definition file.
        /// </summary>
        /// <param name="path"></param>
        public Tileset(FilePath path)
        {
            this.properties = new Dictionary<string, TileProperties>();

            FilePath = path;

            var doc = XDocument.Load(FilePath.Absolute);
            var reader = doc.Element("Tileset");
            if (reader == null)
                throw new Exception("The specified tileset definition file does not contain a Tileset tag.");

            SheetPath = FilePath.FromRelative(reader.Attribute("tilesheet").Value, path.BasePath);

            int size;
            if (!int.TryParse(reader.Attribute("tilesize").Value, out size)) 
                throw new Exception("The tileset definition does not contain a valid tilesize attribute.");
            TileSize = size;

            this.properties["Default"] = TileProperties.Default;
            var propParent = reader.Element("TileProperties");
            if (propParent != null) {
                foreach (XElement propNode in propParent.Elements("Properties")) {
                    var prop = new TileProperties(propNode);
                    this.properties[prop.Name] = prop;
                }
            }

            LoadTilesFromXml(reader);
        }

        private void LoadTilesFromXml(XElement reader) 
        {
            foreach (XElement tileNode in reader.Elements("Tile")) 
            {
                int id = int.Parse(tileNode.Attribute("id").Value);
                string name = tileNode.Attribute("name").Value;
                var sprite = Sprite.Empty;

                var spriteNode = tileNode.Element("Sprite");
                if (spriteNode != null)
                {
                    sprite = Sprite.FromXml(spriteNode);
                }

                Tile tile = new Tile(id, sprite);

                string propName = "Default";
                XAttribute propAttr = tileNode.Attribute("properties");
                if (propAttr != null) 
                    if (this.properties.ContainsKey(propAttr.Value)) 
                        propName = propAttr.Value;

                tile.Properties = this.properties[propName];

                tile.Sprite.Play();
                base.Add(tile);
            }
        }

        // Do not use! Use AddTile instead!
        public new void Add(Tile tile) 
        { 
            throw new NotSupportedException("Don't use this function!"); 
        }

        public void AddTile()
        {
            Sprite sprite = new Sprite(this.TileSize, this.TileSize);
            base.Add(new Tile(this.Count, sprite));
        }

        public TileProperties GetProperties(string name)
        {
            if (properties.ContainsKey(name)) return properties[name];
            return TileProperties.Default;
        }

        public void AddProperties(TileProperties properties)
        {
            this.properties[properties.Name] = properties;
        }

        public void Save(string path)
        {
            if (FilePath == null)
            {
                FilePath = FilePath.FromAbsolute(path, Directory.GetParent(path).FullName);
            }
            else
            {
                FilePath = FilePath.FromAbsolute(path, FilePath.BasePath);
            }
            Save();
        }

        public void Save()
        {
            XmlTextWriter writer = new XmlTextWriter(FilePath.Absolute, null);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartElement("Tileset");
            writer.WriteAttributeString("tilesheet", SheetPath.Relative);
            writer.WriteAttributeString("tilesize", TileSize.ToString());

            writer.WriteStartElement("TileProperties");
            foreach (TileProperties properties in this.properties.Values)
            {
                if (properties.Name == "Default" && properties == TileProperties.Default) 
                    continue;
                properties.Save(writer);
            }
            writer.WriteEndElement();

            foreach (Tile tile in this)
            {
                writer.WriteStartElement("Tile");
                writer.WriteAttributeString("id", tile.Id.ToString());
                writer.WriteAttributeString("name", tile.Name);
                writer.WriteAttributeString("properties", tile.Properties.Name);

                tile.Sprite.WriteTo(writer);

                writer.WriteEndElement();   // end Tile
            }
            writer.WriteEndElement();

            writer.Close();
        }
    }
}
