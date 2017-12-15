using System;
using System.Collections.Generic;
using System.Windows.Input;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Controls;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Bll.Tools
{
    public class TileBrushToolBehavior : IToolBehavior
    {
        private ITileBrush _brush;
        private bool held;
        private Point currentTilePos;
        private int?[,] startTiles;
        private int?[,] endTiles;

        private List<TileChange> changes;

        public TileBrushToolBehavior(ITileBrush brush)
        {
            _brush = brush;
        }

        public void Click(ScreenCanvas canvas, Point location)
        {
            var screen = canvas.Screen;
            changes = new List<TileChange>();

            Point tilePos = new Point(location.X / screen.Tileset.TileSize, location.Y / screen.Tileset.TileSize);

            var selection = screen.Selection;
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

            startTiles = new int?[screen.Width, screen.Height];
            endTiles = new int?[screen.Width, screen.Height];

            // check for line drawing
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.None)
            {
                var xdist = Math.Abs(tilePos.X - currentTilePos.X);
                var ydist = Math.Abs(tilePos.Y - currentTilePos.Y);

                if (xdist >= ydist)
                {
                    var min = Math.Min(currentTilePos.X, tilePos.X);
                    var max = Math.Max(currentTilePos.X, tilePos.X);
                    for (int i = min; i <= max; i++)
                    {
                        Draw(screen, i, currentTilePos.Y);
                    }
                }
                else
                {
                    var min = Math.Min(currentTilePos.Y, tilePos.Y);
                    var max = Math.Max(currentTilePos.Y, tilePos.Y);
                    for (int i = min; i <= max; i++)
                    {
                        Draw(screen, currentTilePos.X, i);
                    }
                }
            }
            else
            {
                Draw(screen, tilePos.X, tilePos.Y);
                held = true;
            }

            currentTilePos = tilePos;
        }

        public void Move(ScreenCanvas canvas, Point location)
        {
            var screen = canvas.Screen;

            if (!held) return;

            Point pos = new Point(location.X / screen.Tileset.TileSize, location.Y / screen.Tileset.TileSize);
            if (pos == currentTilePos) return; // don't keep drawing on the same spot

            Draw(screen, pos.X, pos.Y);
        }

        public void Release(ScreenCanvas canvas, Point location)
        {
            if (startTiles == null) return;

            held = false;

            canvas.Screen.Stage.PushHistoryAction(new DrawAction("Brush", changes));
        }

        public void RightClick(ScreenCanvas surface, Point location)
        {
            int tile_x = location.X / surface.Screen.Tileset.TileSize;
            int tile_y = location.Y / surface.Screen.Tileset.TileSize;

            var tile = surface.Screen.TileAt(tile_x, tile_y);
            ViewModelMediator.Current.GetEvent<TileSelectedEventArgs>().Raise(this, new TileSelectedEventArgs() { Tile = tile });
        }

        private void Draw(ScreenDocument screen, int tile_x, int tile_y)
        {
            screen.BeginDrawBatch();
            var changed = _brush.DrawOn(screen, tile_x, tile_y);
            changes.AddRange(changed);
            screen.EndDrawBatch();
        }

        public bool SuppressContextMenu { get { return true; } }
    }
}
