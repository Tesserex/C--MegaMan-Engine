using System.Collections.Generic;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Bll.Tools
{
    public class BucketToolBehavior : IToolBehavior
    {
        private ITileBrush _brush;
        private readonly int width;
        private readonly int height;
        private readonly List<TileChange> changes;

        public Point IconOffset { get { return Point.Empty; } }
        public bool IconSnap { get { return true; } }
        public bool IsIconCursor { get { return false; } }

        public BucketToolBehavior(ITileBrush brush)
        {
            _brush = brush;
            changes = new List<TileChange>();
            width = brush.Cells.Length;
            height = brush.Cells[0].Length;
        }

        public void Click(ScreenCanvas canvas, Point location)
        {
            int tile_x = location.X / canvas.Screen.Tileset.TileSize;
            int tile_y = location.Y / canvas.Screen.Tileset.TileSize;

            var old = canvas.Screen.TileAt(tile_x, tile_y);

            Flood(canvas, tile_x, tile_y, old.Id, 0, 0);

            canvas.Screen.Stage.PushHistoryAction(new DrawAction("Fill", changes));

            changes.Clear();
        }

        private void Flood(ScreenCanvas canvas, int tile_x, int tile_y, int tile_id, int brush_x, int brush_y)
        {
            var selection = canvas.Screen.Selection;
            if (selection != null)
            {
                // only paint inside selection
                if (!selection.Value.Contains(tile_x, tile_y))
                {
                    return;
                }
            }

            var old = canvas.Screen.TileAt(tile_x, tile_y);
            // checking whether this is already the new tile prevents infinite recursion, but
            // it can prevent filling a solid area with a brush that uses that same tile
            if (old == null || old.Id != tile_id || old.Id == _brush.Cells[brush_x][brush_y].tile.Id) return;

            _brush.DrawOn(canvas.Screen, tile_x, tile_y);
            changes.Add(new TileChange(canvas.Screen, tile_x, tile_y, tile_id, _brush.Cells[brush_x][brush_y].tile.Id));

            Flood(canvas, tile_x - 1, tile_y, tile_id, (brush_x == 0) ? width - 1 : brush_x - 1, brush_y);
            Flood(canvas, tile_x + 1, tile_y, tile_id, (brush_x == width - 1) ? 0 : brush_x + 1, brush_y);
            Flood(canvas, tile_x, tile_y - 1, tile_id, brush_x, (brush_y == 0) ? height - 1 : brush_y - 1);
            Flood(canvas, tile_x, tile_y + 1, tile_id, brush_x, (brush_y == height - 1) ? 0 : brush_y + 1);
        }

        public void Move(ScreenCanvas canvas, Point location)
        {
        }

        public void Release(ScreenCanvas canvas, Point location)
        {
        }

        public void RightClick(ScreenCanvas surface, Point location)
        {
            int tile_x = location.X / surface.Screen.Tileset.TileSize;
            int tile_y = location.Y / surface.Screen.Tileset.TileSize;

            var tile = surface.Screen.TileAt(tile_x, tile_y);
            ViewModelMediator.Current.GetEvent<TileSelectedEventArgs>().Raise(this, new TileSelectedEventArgs() { Tile = tile });
        }

        public bool SuppressContextMenu { get { return true; } }
    }
}