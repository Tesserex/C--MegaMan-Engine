using System.Collections.Generic;
using System.Linq;
using MegaMan.Common;

namespace MegaMan.Editor.Bll.Tools
{
    public interface ITileBrush
    {
        IEnumerable<TileChange> DrawOn(ScreenDocument screen, int tile_x, int tile_y);
        TileBrushCell[][] Cells { get; }
        int Width { get; }
        int Height { get; }
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
        private readonly TileBrushCell[][] _cells;

        public int Height { get { return 1; } }
        public int Width { get { return 1; } }

        public SingleTileBrush(Tile tile)
        {
            this._tile = tile;
            _cells = new TileBrushCell[][]
            {
                new TileBrushCell[]
                {
                    new TileBrushCell()
                    {
                        tile = _tile,
                        x = 0,
                        y = 0
                    }
                }
            };
        }

        public TileBrushCell[][] Cells { get { return _cells; } }

        public virtual IEnumerable<TileChange> DrawOn(ScreenDocument screen, int tile_x, int tile_y)
        {
            var old = screen.TileAt(tile_x, tile_y);

            if (old == null)
            {
                return Enumerable.Empty<TileChange>();
            }

            if (old.Id == _tile.Id)
            {
                return Enumerable.Empty<TileChange>();
            }

            var selection = screen.Selection;
            if (selection != null)
            {
                // only paint inside selection
                if (!selection.Value.Contains(tile_x, tile_y)) return null;
            }

            screen.ChangeTile(tile_x, tile_y, _tile.Id);

            return new[] { new TileChange(screen, tile_x, tile_y, old.Id, _tile.Id) };
        }
    }

    public class MultiTileBrush : ITileBrush
    {
        private TileBrushCell[][] _cells;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public MultiTileBrush(int width, int height)
        {
            Reset(width, height);
        }

        public TileBrushCell[][] Cells { get { return _cells; } }

        public void Reset(int width, int height)
        {
            TileBrushCell[][] newcells = new TileBrushCell[width][];
            for (int i = 0; i < width; i++)
            {
                newcells[i] = new TileBrushCell[height];
                if (_cells != null && i < Width) // old width
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (j < Height) newcells[i][j] = new TileBrushCell(i, j, _cells[i][j].tile);
                    }
                }
            }

            _cells = newcells;
            Width = width;
            Height = height;
        }

        public void AddTile(Tile tile, int x, int y)
        {
            TileBrushCell cell = new TileBrushCell { x = x, y = y, tile = tile };
            _cells[x][y] = cell;
        }

        /// <summary>
        /// Draws the brush onto the given screen at the given tile location.
        /// </summary>
        public IEnumerable<TileChange> DrawOn(ScreenDocument screen, int tile_x, int tile_y)
        {
            List<TileChange> undo = new List<TileChange>();
            bool changed = false;
            var selection = screen.Selection;

            foreach (TileBrushCell[] col in _cells)
            {
                foreach (TileBrushCell cell in col)
                {
                    var old = screen.TileAt(cell.x + tile_x, cell.y + tile_y);

                    if (old == null || old.Id == -1) continue;

                    if (selection != null)
                    {
                        // only paint inside selection
                        if (!selection.Value.Contains(cell.x + tile_x, cell.y + tile_y)) continue;
                    }

                    if (old.Id != cell.tile.Id)
                    {
                        changed = true;
                        screen.ChangeTile(cell.x + tile_x, cell.y + tile_y, cell.tile.Id);
                        undo.Add(new TileChange(screen, tile_x + cell.x, tile_y + cell.y, old.Id, cell.tile.Id));
                    }
                }
            }

            if (changed)
            {
                return undo;
            }

            return Enumerable.Empty<TileChange>();
        }
    }
}
