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
        private int[,] tiles;

        public int BaseX { get; private set; }
        public int BaseY { get; private set; }
        public Tileset Tileset { get; private set; }
        public int Width { get { return tiles.GetLength(0); } }
        public int Height { get { return tiles.GetLength(1); } }
        public int PixelWidth { get { return Width * Tileset.TileSize; } }
        public int PixelHeight { get { return Height * Tileset.TileSize; } }

        public TileLayer(int[,] tiles, Tileset tileset, int base_x, int base_y)
        {
            this.Tileset = tileset;
            this.BaseX = base_x;
            this.BaseY = base_y;
            this.tiles = tiles;
        }

        public Tile TileAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return Tileset[tiles[x,y]];
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
                            s.Write(tiles[x,y].ToString() + " ");
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

            tiles[x,y] = tile;
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

                if (width > this.Width)
                {
                    newOffset.X = width - this.Width;
                }
                else if (width < this.Width)
                {
                    oldOffset.X = this.Width - width;
                }

                if (height > this.Height)
                {
                    newOffset.Y = height - this.Height;
                }
                else if (height < this.Height)
                {
                    oldOffset.Y = this.Height - height;
                }

                CopyOldTiles(width, height, newTiles, oldOffset, newOffset);
            }

            this.tiles = newTiles;
        }

        private void CopyOldTiles(int width, int height, int[,] newTiles, Point oldOffset, Point newOffset)
        {
            // Copy over old tiles
            int minWidth = Math.Min(width, this.Width);
            int minHeight = Math.Min(height, this.Height);
            
            for (int j = 0; j < minHeight; j++)
                for (int i = 0; i < minWidth; i++)
                    newTiles[i + newOffset.X, j + newOffset.Y] = tiles[i + oldOffset.X, j + oldOffset.Y];
        }

        private int[,] CreateNewTiles(int width, int height)
        {
            return new int[width, height];
        }
    }
}
