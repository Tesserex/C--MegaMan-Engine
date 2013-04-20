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

        public FilePath SheetPath { get; set; }

        public FilePath FilePath { get; set; }

        public int TileSize { get; set; }

        public Tileset()
        {
            properties = new Dictionary<string, TileProperties>();
            properties["Default"] = TileProperties.Default;
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
