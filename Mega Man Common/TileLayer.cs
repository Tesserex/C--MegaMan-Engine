﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MegaMan.Common
{
    public class TileLayer
    {
        private int[][] tiles;
        private int base_x;
        private int base_y;

        public Tileset Tileset { get; private set; }
        public int Width { get { return tiles[0].Length; } }
        public int Height { get { return tiles.GetLength(0); } }
        public int PixelWidth { get { return tiles[0].Length * Tileset.TileSize; } }
        public int PixelHeight { get { return tiles.GetLength(0) * Tileset.TileSize; } }

        public TileLayer(int[][] tiles, Tileset tileset, int base_x, int base_y)
        {
            this.Tileset = tileset;
            this.base_x = base_x;
            this.base_y = base_y;
            this.tiles = tiles;
        }

        public Tile TileAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return Tileset[tiles[y][x]];
        }

        public int? TileIndexAt(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return tiles[y][x];
        }

        public void Draw(SpriteBatch batch, Microsoft.Xna.Framework.Color color, float off_x, float off_y, int width, int height)
        {
            if (Tileset == null)
                throw new InvalidOperationException("Screen has no tileset to draw with.");

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float xpos = x * Tileset.TileSize + off_x + base_x;
                    float ypos = y * Tileset.TileSize + off_y + base_y;

                    if (xpos + Tileset.TileSize < 0 || ypos + Tileset.TileSize < 0) continue;
                    if (xpos > width || ypos > height) continue;
                    Tileset[tiles[y][x]].Draw(batch, color, xpos, ypos);
                }
            }
        }

        public void Draw(Graphics g, float off_x, float off_y, int width, int height, Func<Image, Image> transform)
        {
            if (Tileset == null)
                throw new InvalidOperationException("Screen has no tileset to draw with.");

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float xpos = x * Tileset.TileSize + off_x;
                    float ypos = y * Tileset.TileSize + off_y;

                    if (xpos + Tileset.TileSize < 0 || ypos + Tileset.TileSize < 0) continue;
                    if (xpos > width || ypos > height) continue;
                    Tileset[tiles[y][x]].Draw(g, xpos, ypos, transform);
                }
            }
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
                CopyOldTiles(width, height, newTiles);

            this.tiles = newTiles;
        }

        private void CopyOldTiles(int width, int height, int[][] newTiles)
        {
            // Copy over old tiles
            int minWidth = (width < tiles[0].Length) ? width : tiles[0].Length;
            int minHeight = (height < tiles.Length) ? height : tiles.Length;

            for (int j = 0; j < minHeight; j++)
                for (int i = 0; i < minWidth; i++)
                    newTiles[j][i] = tiles[j][i];
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
