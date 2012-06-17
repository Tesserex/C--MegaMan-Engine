using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Xml.Linq;
using System.IO;
using System.Xml;

namespace MegaMan.Common
{
    public class Tileset : List<Tile>
    {
        private Dictionary<string, TileProperties> properties;
        public IEnumerable<TileProperties> Properties { get { return properties.Values; } }

        public Image Sheet { get; private set; }

        private string sheetPathAbs, sheetPathRel;

        public string SheetPathAbs
        {
            get { return sheetPathAbs; }
            set
            {
                sheetPathAbs = Path.GetFullPath(value);
                if (!string.IsNullOrEmpty(filePathAbs))
                {
                    sheetPathRel = StageInfo.PathToRelative(sheetPathAbs, filePathAbs);
                }
            }
        }

        private string filePathAbs;

        // this is inherently an absolute path.
        // If you asked "what about relative?",
        // I would reply, "relative to what?"
        public string FilePath
        {
            get { return filePathAbs; }
            private set
            {
                filePathAbs = Path.GetFullPath(value);
                if (!string.IsNullOrEmpty(SheetPathAbs))
                {
                    sheetPathRel = StageInfo.PathToRelative(SheetPathAbs, filePathAbs);
                }
            }
        }

        public int TileSize { get; private set; }

        // ************
        // Constructors
        // ************

        // Creates a new Tileset from the given image with tiles of the specified size.
        public Tileset(Image sheet, int tilesize)
        {
            this.TileSize = tilesize;
            this.Sheet = sheet;
            this.properties = new Dictionary<string, TileProperties>();
            this.properties["Default"] = TileProperties.Default;
        }

        /// <summary>
        /// Construct a Tileset by specifying an absolute path to a tileset XML definition file.
        /// </summary>
        /// <param name="path"></param>
        public Tileset(string path)
        {
            this.properties = new Dictionary<string, TileProperties>();

            FilePath = path;

            var doc = XDocument.Load(FilePath);
            var reader = doc.Element("Tileset");
            if (reader == null)
                throw new Exception("The specified tileset definition file does not contain a Tileset tag.");

            string sheetDirectory = Directory.GetParent(FilePath).FullName;
            
            // this may seem redundant, combining, and then having the property split it
            // but it protects from the case where the given path was accidentally absolute
            // this will then set the relative sheet path automatically
            SheetPathAbs = Path.Combine(sheetDirectory, reader.Attribute("tilesheet").Value);

            try 
            {
                Sheet = Bitmap.FromFile(SheetPathAbs);
            } 
            catch (FileNotFoundException err) 
            {
                throw new Exception("A tile image file was not found at the location specified by the tileset definition: " + SheetPathAbs, err);
            }

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

        public void LoadTilesFromXml(XElement reader) 
        {
            foreach (XElement tileNode in reader.Elements("Tile")) 
            {
                int id = int.Parse(tileNode.Attribute("id").Value);
                string name = tileNode.Attribute("name").Value;
                var sprite = Sprite.Empty;

                var spriteNode = tileNode.Element("Sprite");
                if (spriteNode != null) 
                    sprite = Sprite.FromXml(spriteNode, Sheet);

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


        public void SetTextures(Microsoft.Xna.Framework.Graphics.GraphicsDevice device)
        {
            foreach (Tile tile in this)
            {
                tile.Sprite.SetTexture(device, this.sheetPathAbs);
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
            sprite.sheet = this.Sheet;
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
            FilePath = path; // this will adjust sheet paths accordingly
            Save();
        }

        public void Save()
        {
            XmlTextWriter writer = new XmlTextWriter(FilePath, null);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 1;
            writer.IndentChar = '\t';

            writer.WriteStartElement("Tileset");
            writer.WriteAttributeString("tilesheet", sheetPathRel);
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
