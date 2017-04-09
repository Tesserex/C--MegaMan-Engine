using System;
using System.Collections.Generic;
using System.Linq;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Bll.Tools
{
    public class RectangleToolBehavior : IToolBehavior
    {
        private readonly ITileBrush brush;
        private int tx1, ty1, tx2, ty2;
        private bool held;
        private int?[,] startTiles;
        private int?[,] endTiles;

        public RectangleToolBehavior(ITileBrush brush)
        {
            this.brush = brush;
            held = false;
        }

        public void Click(ScreenCanvas surface, Point location)
        {
            tx1 = location.X / surface.Screen.Tileset.TileSize;
            ty1 = location.Y / surface.Screen.Tileset.TileSize;
            tx2 = tx1;
            ty2 = ty1;
            held = true;
        }

        public void Move(ScreenCanvas surface, Point location)
        {
            if (held)
            {
                tx2 = (int)Math.Round(location.X / (float)surface.Screen.Tileset.TileSize);
                ty2 = (int)Math.Round(location.Y / (float)surface.Screen.Tileset.TileSize);

                SetSelection(surface);
            }
        }

        private void SetSelection(ScreenCanvas surface)
        {
            surface.Screen.SetSelection(
                Math.Min(tx1, tx2),
                Math.Min(ty1, ty2),
                Math.Abs(tx2 - tx1),
                Math.Abs(ty2 - ty1)
            );
        }

        public void Release(ScreenCanvas canvas, Point location)
        {
            held = false;

            int x_start = Math.Min(tx1, tx2);
            int x_end = Math.Max(tx1, tx2) - 1;
            int y_start = Math.Min(ty1, ty2);
            int y_end = Math.Max(ty1, ty2) - 1;

            startTiles = new int?[canvas.Screen.Width, canvas.Screen.Height];
            endTiles = new int?[canvas.Screen.Width, canvas.Screen.Height];

            for (int y = y_start; y <= y_end; y += brush.Cells[0].Length)
            {
                for (int x = x_start; x <= x_end; x += brush.Cells.Length)
                {
                    Draw(canvas, x, y);
                }
            }

            var changes = new List<TileChange>();

            for (int y = 0; y < canvas.Screen.Height; y++)
            {
                for (int x = 0; x < canvas.Screen.Width; x++)
                {
                    if (startTiles[x, y].HasValue && endTiles[x, y].HasValue && startTiles[x, y].Value != endTiles[x, y].Value)
                    {
                        changes.Add(new TileChange(canvas.Screen, x, y, startTiles[x, y].Value, endTiles[x, y].Value));
                    }
                }
            }

            canvas.Screen.SetSelection(0, 0, 0, 0);
        }

        private void Draw(ScreenCanvas surface, int tile_x, int tile_y)
        {
            // first track the changes i'm going to make for undo purposes
            foreach (TileBrushCell cell in brush.Cells.SelectMany(c => c))
            {
                int tx = cell.x + tile_x;
                int ty = cell.y + tile_y;

                if (tx < 0 || tx >= surface.Screen.Width || ty < 0 || ty >= surface.Screen.Height) continue;

                if (startTiles[tx, ty] == null) // don't overwrite existing data
                {
                    startTiles[tx, ty] = surface.Screen.TileAt(tx, ty).Id;
                }

                endTiles[tx, ty] = cell.tile.Id;
            }

            brush.DrawOn(surface.Screen, tile_x, tile_y);
        }

        public void RightClick(ScreenCanvas surface, Point location)
        {

        }

        public bool SuppressContextMenu { get { return false; } }
    }
}
