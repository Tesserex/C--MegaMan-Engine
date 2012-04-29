using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;

namespace MegaMan.LevelEditor
{
    public class RectangleTool : ITool
    {
        private readonly ITileBrush brush;
        private int tx1, ty1, tx2, ty2;
        private bool held;
        private int?[,] startTiles;
        private int?[,] endTiles;

        public RectangleTool(ITileBrush brush)
        {
            this.brush = brush;
            held = false;
        }

        public Image Icon
        {
            get { return Properties.Resources.cross; }
        }

        public bool IconSnap
        {
            get { return true; }
        }

        public bool IsIconCursor
        {
            get { return false; }
        }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            tx1 = location.X / surface.Screen.Tileset.TileSize;
            ty1 = location.Y / surface.Screen.Tileset.TileSize;
            held = true;
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
            if (held)
            {
                tx2 = location.X / surface.Screen.Tileset.TileSize;
                ty2 = location.Y / surface.Screen.Tileset.TileSize;

                var g = surface.GetToolLayerGraphics();
                if (g != null)
                {
                    g.Clear(Color.Transparent);

                    // draw rectangle preview
                    int x_start = Math.Min(tx1, tx2);
                    int x_end = Math.Max(tx1, tx2) - 1;
                    int y_start = Math.Min(ty1, ty2);
                    int y_end = Math.Max(ty1, ty2) - 1;

                    for (int y = y_start; y <= y_end; y += brush.Height)
                    {
                        for (int x = x_start; x <= x_end; x += brush.Width)
                        {
                            brush.DrawOn(g, x * brush.CellSize, y * brush.CellSize);
                        }
                    }

                    surface.ReturnToolLayerGraphics(g);
                }
            }
        }

        public void Release(ScreenDrawingSurface surface)
        {
            held = false;

            var g = surface.GetToolLayerGraphics();
            if (g != null)
            {
                g.Clear(Color.Transparent);
                surface.ReturnToolLayerGraphics(g);
            }

            int x_start = Math.Min(tx1, tx2);
            int x_end = Math.Max(tx1, tx2) - 1;
            int y_start = Math.Min(ty1, ty2);
            int y_end = Math.Max(ty1, ty2) - 1;

            startTiles = new int?[surface.Screen.Width, surface.Screen.Height];
            endTiles = new int?[surface.Screen.Width, surface.Screen.Height];

            for (int y = y_start; y <= y_end; y += brush.Height)
            {
                for (int x = x_start; x <= x_end; x += brush.Width)
                {
                    Draw(surface, x, y);
                }
            }

            var changes = new List<TileChange>();

            for (int y = 0; y < surface.Screen.Height; y++)
            {
                for (int x = 0; x < surface.Screen.Width; x++)
                {
                    if (startTiles[x, y].HasValue && endTiles[x, y].HasValue && startTiles[x, y].Value != endTiles[x, y].Value)
                    {
                        changes.Add(new TileChange(x, y, startTiles[x, y].Value, endTiles[x, y].Value, surface));
                    }
                }
            }

            if (changes.Count > 0) surface.EditedWithAction(new DrawAction("Rectangle", changes, surface));
        }

        private void Draw(ScreenDrawingSurface surface, int tile_x, int tile_y)
        {
            // first track the changes i'm going to make for undo purposes
            foreach (TileBrushCell cell in brush.Cells())
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

            surface.ReDrawTiles();
        }

        public void RightClick(ScreenDrawingSurface surface, Point location)
        {
            
        }

        public Point IconOffset
        {
            get { return new Point(-7, -7); }
        }
    }
}
