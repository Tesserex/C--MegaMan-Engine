using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MegaMan.Common
{
    public class TileLayer
    {
        private Screen parentScreen;
        private int[][] tiles;
        private int base_x;
        private int base_y;

        public int Width { get { return tiles[0].Length; } }
        public int Height { get { return tiles.GetLength(0); } }
        public int PixelWidth { get { return tiles[0].Length * parentScreen.Tileset.TileSize; } }
        public int PixelHeight { get { return tiles.GetLength(0) * parentScreen.Tileset.TileSize; } }

        public TileLayer(string filepath, Screen screen, int base_x, int base_y)
        {
            this.parentScreen = screen;
            this.base_x = base_x;
            this.base_y = base_y;

            string[] lines = File.ReadAllLines(filepath);
            string[] firstline = lines[0].Split(' ');
            int width = int.Parse(firstline[0]);
            int height = int.Parse(firstline[1]);
            tiles = new int[height][];
            for (int y = 0; y < height; y++)
            {
                tiles[y] = new int[width];
                string[] line = lines[y + 1].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                for (int x = 0; x < width; x++)
                {
                    int id = int.Parse(line[x]);
                    tiles[y][x] = id;
                }
            }
        }

        public Tile TileAt(int x, int y)
        {
            x -= base_x;
            y -= base_y;

            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return parentScreen.Tileset[tiles[y][x]];
        }

        public int? TileIndexAt(int x, int y)
        {
            x -= base_x;
            y -= base_y;

            if (x < 0 || y < 0 || x >= Width || y >= Height) return null;
            return tiles[y][x];
        }

        public void Draw(SpriteBatch batch, Color color, float off_x, float off_y, int width, int height)
        {
            if (parentScreen.Tileset == null)
                throw new InvalidOperationException("Screen has no tileset to draw with.");

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float xpos = x * parentScreen.Tileset.TileSize + off_x + base_x;
                    float ypos = y * parentScreen.Tileset.TileSize + off_y + base_y;

                    if (xpos + parentScreen.Tileset.TileSize < 0 || ypos + parentScreen.Tileset.TileSize < 0) continue;
                    if (xpos > width || ypos > height) continue;
                    parentScreen.Tileset[tiles[y][x]].Draw(batch, color, xpos, ypos);
                }
            }
        }
    }
}
