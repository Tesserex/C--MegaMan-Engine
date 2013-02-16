using MegaMan.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Editor.Bll.Tools
{
    public interface ITileBrush
    {
        ITileBrush DrawOn(ScreenDocument screen, int tile_x, int tile_y);
    }

    public struct TileBrushCell
    {
        public int x;
        public int y;
        public Tile tile;

        public TileBrushCell(int x, int y, Tile tile)
        {
            this.x = x;
            this.y = y;
            this.tile = tile;
        }
    }

    public class SingleTileBrush : ITileBrush
    {
        private readonly Tile _tile;

        public int Height { get { return 1; } }
        public int Width { get { return 1; } }

        public SingleTileBrush(Tile tile)
        {
            this._tile = tile;
        }

        public virtual ITileBrush DrawOn(ScreenDocument screen, int tile_x, int tile_y)
        {
            var old = screen.TileAt(tile_x, tile_y);
            
            if (old == null)
            {
                return null;
            }

            if (old.Id == _tile.Id)
            {
                return null;
            }

            var selection = screen.Selection;
            if (selection != null)
            {
                // only paint inside selection
                if (!selection.Value.Contains(tile_x, tile_y)) return null;
            }

            screen.ChangeTile(tile_x, tile_y, _tile.Id);

            return new SingleTileBrush(old);
        }
    }

    public class MultiTileBrush : ITileBrush
    {
        private TileBrushCell[][] _cells;
        private int _width;
        private int _height;

        public MultiTileBrush(int width, int height)
        {
            Reset(width, height);
        }

        public void Reset(int width, int height)
        {
            TileBrushCell[][] newcells = new TileBrushCell[width][];
            for (int i = 0; i < width; i++)
            {
                newcells[i] = new TileBrushCell[height];
                if (_cells != null && i < _width) // old width
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (j < _height) newcells[i][j] = new TileBrushCell(i, j, _cells[i][j].tile);
                    }
                }
            }

            _cells = newcells;
            _width = width;
            _height = height;
        }

        public void AddTile(Tile tile, int x, int y)
        {
            TileBrushCell cell = new TileBrushCell {x = x, y = y, tile = tile};
            _cells[x][y] = cell;
        }

        /// <summary>
        /// Draws the brush onto the given screen at the given tile location.
        /// Returns an "undo brush" - a brush of all tiles that were overwritten.
        /// Returns null if no tiles were changed.
        /// </summary>
        public ITileBrush DrawOn(ScreenDocument screen, int tile_x, int tile_y)
        {
            MultiTileBrush undo = new MultiTileBrush(_width, _height);
            bool changed = false;
            foreach (TileBrushCell[] col in _cells)
            {
                foreach (TileBrushCell cell in col)
                {
                    var old = screen.TileAt(cell.x + tile_x, cell.y + tile_y);

                    if (old == null) continue;

                    undo.AddTile(old, cell.x, cell.y);
                    if (old.Id != cell.tile.Id)
                    {
                        changed = true;
                        screen.ChangeTile(cell.x + tile_x, cell.y + tile_y, cell.tile.Id);
                    }
                }
            }

            if (changed)
            {
                return undo;
            }

            return null;
        }
    }
}
