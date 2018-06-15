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
            Tileset = tileset;
            BaseX = base_x;
            BaseY = base_y;
            this.tiles = tiles;
        }

        public Tile TileAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return Tileset.SingleOrDefault(t => t.Id == tiles[x,y]);
        }

        public TileLayer Clone()
        {
            return new TileLayer((int[,])tiles.Clone(), Tileset, BaseX, BaseY);
        }

        public void Save(string filepath)
        {
            using (var f = File.Open(filepath, FileMode.Create))
            {
                using (var s = new StreamWriter(f))
                {
                    s.WriteLine(Width + " " + Height);

                    for (var y = 0; y < Height; y++)
                    {
                        for (var x = 0; x < Width; x++)
                        {
                            s.Write(tiles[x,y] + " ");
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

            if (!Tileset.Any(t => t.Id == tile) && tile != -1)
                return;

            tiles[x,y] = tile;
        }

        public void ChangeTiles(Point offset, int[,] newTiles)
        {
            var minWidth = Math.Min(newTiles.GetLength(0), Width - offset.X);
            var minHeight = Math.Min(newTiles.GetLength(1), Height - offset.Y);

            for (var x = 0; x < minWidth; x++)
            {
                for (var y = 0; y < minHeight; y++)
                {
                    tiles[x + offset.X, y + offset.Y] = newTiles[x, y];
                }
            }
        }

        public int[,] GetTiles(Point offset, int width, int height)
        {
            width = Math.Min(width, Width - offset.X);
            height = Math.Min(height, Height - offset.Y);

            var tileBuffer = new int[width, height];

            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    tileBuffer[x, y] = tiles[x + offset.X, y + offset.Y];
                }
            }

            return tileBuffer;
        }

        public void Resize(int width, int height)
        {
            var currentTileBuffer = tiles;

            tiles = CreateNewTiles(width, height);

            ChangeTiles(Point.Empty, currentTileBuffer);
        }

        public void ResizeTopLeft(int width, int height)
        {
            var bufferOffset = new Point(Math.Max(0, Width - width), Math.Max(0, Height - height));
            var placementOffset = new Point(Math.Max(0, width - Width), Math.Max(0, height - Height));

            var currentTileBuffer = GetTiles(bufferOffset, Width, Height);

            tiles = CreateNewTiles(width, height);

            ChangeTiles(placementOffset, currentTileBuffer);
        }

        private int[,] CreateNewTiles(int width, int height)
        {
            return new int[width, height];
        }
    }
}
