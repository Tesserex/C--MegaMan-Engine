using System.Collections.Generic;
using System.Drawing;
using MegaMan.Common;

namespace MegaMan.LevelEditor
{
    public class Bucket : ITool
    {
        private readonly Tile[,] cells;
        private readonly int width;
        private readonly int height;
        private readonly List<TileChange> changes;

        public Image Icon { get; private set; }
        public Point IconOffset { get { return Point.Empty; } }
        public bool IconSnap { get { return true; } }
        public bool IsIconCursor { get { return false; } }

        public Bucket(ITileBrush brush)
        {
            width = brush.Width;
            height = brush.Height;
            cells = new Tile[width, height];
            foreach (TileBrushCell cell in brush.Cells())
            {
                cells[cell.x, cell.y] = cell.tile;
            }
            Icon = new Bitmap(brush.Width * brush.CellSize, brush.Height * brush.CellSize);
            using (Graphics g = Graphics.FromImage(Icon))
            {
                brush.DrawOn(g, 0, 0);
            }
            changes = new List<TileChange>();
        }

        public void Click(ScreenDrawingSurface surface, Point location)
        {
            var selection = surface.Selection;
            if (selection != null)
            {
                // only paint inside selection
                if (!selection.Value.Contains(location))
                {
                    return;
                }
            }

            int tile_x = location.X / surface.Screen.Tileset.TileSize;
            int tile_y = location.Y / surface.Screen.Tileset.TileSize;

            var old = surface.Screen.TileAt(tile_x, tile_y);

            Flood(surface, tile_x, tile_y, old.Id, 0, 0);

            // need to manually inform the screen surface that I messed with it
            if (changes.Count > 0)
            {
                surface.EditedWithAction(new DrawAction("Fill", changes, surface));
                surface.ReDrawTiles();
            }
            changes.Clear();
        }

        private void Flood(ScreenDrawingSurface surface, int tile_x, int tile_y, int tile_id, int brush_x, int brush_y)
        {
            var selection = surface.Selection;
            if (selection != null)
            {
                // only paint inside selection
                if (!selection.Value.Contains(tile_x, tile_y))
                {
                    return;
                }
            }

            var old = surface.Screen.TileAt(tile_x, tile_y);
            // checking whether this is already the new tile prevents infinite recursion, but
            // it can prevent filling a solid area with a brush that uses that same tile
            if (old == null || old.Id != tile_id || old.Id == cells[brush_x, brush_y].Id) return;

            surface.Screen.ChangeTile(tile_x, tile_y, cells[brush_x, brush_y].Id);
            changes.Add(new TileChange(tile_x, tile_y, tile_id, cells[brush_x, brush_y].Id, surface));

            Flood(surface, tile_x - 1, tile_y, tile_id, (brush_x == 0)? width-1 : brush_x - 1, brush_y);
            Flood(surface, tile_x + 1, tile_y, tile_id, (brush_x == width - 1) ? 0 : brush_x + 1, brush_y);
            Flood(surface, tile_x, tile_y - 1, tile_id, brush_x, (brush_y == 0) ? height - 1 : brush_y - 1);
            Flood(surface, tile_x, tile_y + 1, tile_id, brush_x, (brush_y == height - 1) ? 0 : brush_y + 1);
        }

        public void Move(ScreenDrawingSurface surface, Point location)
        {
        }

        public void Release(ScreenDrawingSurface surface)
        {
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