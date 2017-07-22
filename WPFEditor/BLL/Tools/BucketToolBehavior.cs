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

        public bool IsGlobal { get; set; }

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

            if (IsGlobal)
                Global(canvas, tile_x, tile_y, old.Id);
            else
                Flood(canvas, tile_x, tile_y, old.Id, 0, 0);

            canvas.Screen.Stage.PushHistoryAction(new DrawAction("Fill", changes));

            changes.Clear();
        }

        private void Flood(ScreenCanvas canvas, int tile_x, int tile_y, int previousTileId, int brush_x, int brush_y)
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
            if (old == null || old.Id == -1 || old.Id != previousTileId) return;

            var changed = _brush.DrawCellOn(canvas.Screen, tile_x, tile_y, brush_x, brush_y);
            changes.AddRange(changed);

            Flood(canvas, tile_x - 1, tile_y, previousTileId, (brush_x == 0) ? width - 1 : brush_x - 1, brush_y);
            Flood(canvas, tile_x + 1, tile_y, previousTileId, (brush_x == width - 1) ? 0 : brush_x + 1, brush_y);
            Flood(canvas, tile_x, tile_y - 1, previousTileId, brush_x, (brush_y == 0) ? height - 1 : brush_y - 1);
            Flood(canvas, tile_x, tile_y + 1, previousTileId, brush_x, (brush_y == height - 1) ? 0 : brush_y + 1);
        }

        private void Global(ScreenCanvas canvas, int tile_x, int tile_y, int previousTileId)
        {
            var brush_x0 = (_brush.Width - (tile_x % _brush.Width)) % _brush.Width;
            var brush_y = (_brush.Height - (tile_y % _brush.Height)) % _brush.Height;

            var brush_x = brush_x0;

            for (var y = 0; y < canvas.Screen.Height; y++)
            {
                for (var x = 0; x < canvas.Screen.Width; x++)
                {
                    var old = canvas.Screen.TileAt(x, y);
                    if (old != null && old.Id == previousTileId)
                    {
                        var changed = _brush.DrawCellOn(canvas.Screen, x, y, brush_x, brush_y);
                        changes.AddRange(changed);
                    }

                    brush_x = (brush_x + 1) % _brush.Width;
                }
                brush_y = (brush_y + 1) % _brush.Height;
                brush_x = brush_x0;
            }
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