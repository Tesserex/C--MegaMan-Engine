﻿using System.Collections.Generic;
using System.Linq;

namespace MegaMan.Common
{
    public class Tileset : List<Tile>
    {
        private readonly Dictionary<string, TileProperties> properties;
        public IEnumerable<TileProperties> Properties { get { return properties.Values; } }

        public FilePath SheetPath { get; set; }

        public FilePath FilePath { get; set; }

        public int TileSize { get; set; }

        public Tileset()
        {
            properties = new Dictionary<string, TileProperties> {
                ["Default"] = TileProperties.Default
            };
        }

        public void ChangeSheetPath(string path)
        {
            SheetPath = FilePath.FromAbsolute(path, FilePath.BasePath);
        }

        public Tile AddTile()
        {
            var nextId = this.Any() ? this.Max(t => t.Id) + 1 : 1;
            var sprite = new TileSprite(this);
            var tile = new Tile(nextId, sprite);
            Add(tile);
            return tile;
        }

        public TileProperties GetProperties(string name)
        {
            if (properties.ContainsKey(name)) return properties[name];
            return TileProperties.Default;
        }

        public void AddProperties(TileProperties props)
        {
            properties[props.Name] = props;
        }

        public void DeleteProperties(TileProperties props)
        {
            foreach (var tile in this.Where(t => t.Properties == props))
            {
                tile.Properties = TileProperties.Default;
            }

            properties.Remove(props.Name);
        }
    }
}
