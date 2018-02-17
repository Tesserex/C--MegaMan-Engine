using System;
using System.IO;
using System.Linq;
using MegaMan.Common.Geometry;

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
            return Tileset.SingleOrDefault(t => t.Id == tiles[x,y]);
        }

        public TileLayer Clone()
        {
            return new TileLayer((int[,])this.tiles.Clone(), this.Tileset, this.BaseX, this.BaseY);
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

            if (!Tileset.Any(t => t.Id == tile))
                return;

            tiles[x,y] = tile;
        }

        public void ChangeTiles(Point offset, int[,] newTiles)
        {
            int minWidth = Math.Min(newTiles.GetLength(0), this.Width - offset.X);
            int minHeight = Math.Min(newTiles.GetLength(1), this.Height - offset.Y);

            for (int x = 0; x < minWidth; x++)
            {
                for (int y = 0; y < minHeight; y++)
                {
                    this.tiles[x + offset.X, y + offset.Y] = newTiles[x, y];
                }
            }
        }

        public int[,] GetTiles(Point offset, int width, int height)
        {
            width = Math.Min(width, this.Width - offset.X);
            height = Math.Min(height, this.Height - offset.Y);

            var tileBuffer = new int[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    tileBuffer[x, y] = this.tiles[x + offset.X, y + offset.Y];
                }
            }

            return tileBuffer;
        }

        public void Resize(int width, int height)
        {
            var currentTileBuffer = this.tiles;

            this.tiles = CreateNewTiles(width, height);

            ChangeTiles(Point.Empty, currentTileBuffer);
        }

        public void ResizeTopLeft(int width, int height)
        {
            var bufferOffset = new Point(Math.Max(0, this.Width - width), Math.Max(0, this.Height - height));
            var placementOffset = new Point(Math.Max(0, width - this.Width), Math.Max(0, height - this.Height));

            var currentTileBuffer = GetTiles(bufferOffset, this.Width, this.Height);

            this.tiles = CreateNewTiles(width, height);

            ChangeTiles(placementOffset, currentTileBuffer);
        }

        private int[,] CreateNewTiles(int width, int height)
        {
            return new int[width, height];
        }
    }
}
