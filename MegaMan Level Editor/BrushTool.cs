using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System;

namespace MegaMan.LevelEditor
{
    public class BrushTool : ITool
    {
        private readonly ITileBrush brush;
        private bool held;
        private Point currentTilePos;
        private int?[,] startTiles;
        private int?[,] endTiles;

        public Image Icon { get; private set; }
        public Point IconOffset { get { return Point.Empty; } }
        public bool IconSnap { get { return true; } }
        public bool IsIconCursor { get { return false; } }

        public BrushTool(ITileBrush brush)
        {
            this.brush = brush;
            held = false;
            Icon = new Bitmap(brush.Width * brush.CellSize, brush.Height * brush.CellSize);
            using (Graphics g = Graphics.FromImage(Icon))
            {
                brush.DrawOn(g, 0, 0);
            }
        }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            Point tilePos = new Point(location.X / surface.Screen.Tileset.TileSize, location.Y / surface.Screen.Tileset.TileSize);

            var selection = surface.Selection;
            if (selection != null)
            {
                // only paint inside selection
                if (!selection.Value.Contains(tilePos))
                {
                    startTiles = null;
                    endTiles = null;
                    return;
                }
            }

            startTiles = new int?[surface.Screen.Width, surface.Screen.Height];
            endTiles = new int?[surface.Screen.Width, surface.Screen.Height];

            // check for line drawing
            if ((Control.ModifierKeys & Keys.Shift) != Keys.None)
            {
                var xdist = Math.Abs(tilePos.X - currentTilePos.X);
                var ydist = Math.Abs(tilePos.Y - currentTilePos.Y);

                if (xdist >= ydist)
                {
                    var min = Math.Min(currentTilePos.X, tilePos.X);
                    var max = Math.Max(currentTilePos.X, tilePos.X);
                    for (int i = min; i <= max; i += brush.Width)
                    {
                        Draw(surface, i, currentTilePos.Y);
                    }
                }
                else
                {
                    var min = Math.Min(currentTilePos.Y, tilePos.Y);
                    var max = Math.Max(currentTilePos.Y, tilePos.Y);
                    for (int i = min; i <= max; i += brush.Height)
                    {
                        Draw(surface, currentTilePos.X, i);
                    }
                }
            }
            else
            {
                Draw(surface, tilePos.X, tilePos.Y);
                held = true;
            }

            currentTilePos = tilePos;
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
            if (!held) return;
            Point pos = new Point(location.X / surface.Screen.Tileset.TileSize, location.Y / surface.Screen.Tileset.TileSize);
            if (pos == currentTilePos) return; // don't keep drawing on the same spot

            Draw(surface, pos.X, pos.Y);
        }

        public void Release(ScreenDrawingSurface surface)
        {
            if (startTiles == null) return;

            held = false;
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
            
            if (changes.Count > 0) surface.EditedWithAction(new DrawAction("Brush", changes, surface));
            changes.Clear();
        }

        private void Draw(ScreenDrawingSurface surface, int tile_x, int tile_y)
        {
            var selection = surface.Selection;
            if (selection != null)
            {
                // only paint inside selection
                if (!selection.Value.Contains(tile_x, tile_y)) return;
            }

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

        // behaves as eyedropper
        public void RightClick(ScreenDrawingSurface surface, Point location)
        {
            int tile_x = location.X / surface.Screen.Tileset.TileSize;
            int tile_y = location.Y / surface.Screen.Tileset.TileSize;

            var tile = surface.Screen.TileAt(tile_x, tile_y);
            MainForm.Instance.TileStrip.SelectTile(tile);
        }
    }
}