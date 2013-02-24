using MegaMan.Common.Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace MegaMan.Common
{
    public class TileLayer
    {
        private int[][] tiles;

        public int BaseX { get; private set; }
        public int BaseY { get; private set; }
        public Tileset Tileset { get; private set; }
        public int Width { get { return tiles[0].Length; } }
        public int Height { get { return tiles.GetLength(0); } }
        public int PixelWidth { get { return tiles[0].Length * Tileset.TileSize; } }
        public int PixelHeight { get { return tiles.GetLength(0) * Tileset.TileSize; } }

        public TileLayer(int[][] tiles, Tileset tileset, int base_x, int base_y)
        {
            this.Tileset = tileset;
            this.BaseX = base_x;
            this.BaseY = base_y;
            this.tiles = tiles;
        }

        public Tile TileAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return Tileset[tiles[y][x]];
        }

        public void Save(string filepath)
        {
            using (FileStream f = File.Open(filepath, FileMode.Create))
            {
                using (StreamWriter s = new StreamWriter(f))
                {
                    s.WriteLine(Width.ToString() + " " + Height.ToString());

                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            s.Write(tiles[y][x].ToString() + " ");
                        }
                        s.Write('\n');
                    }
                }
                f.Close();
            }
        }

        public void ChangeTile(int x, int y, int tile)
        {
            if (y < 0 || y >= Height || x < 0 || x >= Width)
                return;

            if (tile < 0 || tile >= Tileset.Count)
                throw new ArgumentException("Tile is not within tileset range");

            tiles[y][x] = tile;
        }

        public void Resize(int width, int height)
        {
            var newTiles = CreateNewTiles(width, height);

            if (this.tiles != null)
                CopyOldTiles(width, height, newTiles, Point.Empty, Point.Empty);

            this.tiles = newTiles;
        }

        public void ResizeTopLeft(int width, int height)
        {
            var newTiles = CreateNewTiles(width, height);

            if (this.tiles != null)
            {
                var oldOffset = Point.Empty;
                var newOffset = Point.Empty;

                if (width > tiles[0].Length)
                {
                    newOffset.X = width - tiles[0].Length;
                }
                else if (width < tiles[0].Length)
                {
                    oldOffset.X = tiles[0].Length - width;
                }

                if (height > tiles.Length)
                {
                    newOffset.Y = height - tiles.Length;
                }
                else if (height < tiles.Length)
                {
                    oldOffset.Y = tiles.Length - height;
                }

                CopyOldTiles(width, height, newTiles, oldOffset, newOffset);
            }

            this.tiles = newTiles;
        }

        private void CopyOldTiles(int width, int height, int[][] newTiles, Point oldOffset, Point newOffset)
        {
            // Copy over old tiles
            int minWidth = Math.Min(width, tiles[0].Length);
            int minHeight = Math.Min(height, tiles.Length);

            for (int j = 0; j < minHeight; j++)
                for (int i = 0; i < minWidth; i++)
                    newTiles[j + newOffset.Y][i + newOffset.X] = tiles[j + oldOffset.Y][i + oldOffset.X];
        }

        private int[][] CreateNewTiles(int width, int height)
        {
            var newTiles = new int[height][];
            for (int y = 0; y < height; y++)
            {
                newTiles[y] = new int[width];
                for (int x = 0; x < width; x++)
                    newTiles[y][x] = 0;
            }
            return newTiles;
        }
    }
}
