using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using MegaMan.Common;

namespace MegaMan.LevelEditor
{
    public interface ITileBrush
    {
        ITileBrush DrawOn(ScreenDocument screen, int tile_x, int tile_y);
        void DrawOn(Graphics g, int x, int y);
        IEnumerable<TileBrushCell> Cells();
        int Height { get; }
        int Width { get; }
        int CellSize { get; }
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
        private readonly Tile tile;

        public int Height { get { return 1; } }
        public int Width { get { return 1; } }

        public int CellSize { get { return (int)tile.Width; } }

        public SingleTileBrush(Tile tile)
        {
            this.tile = tile;
        }

        public void DrawOn(Graphics g, int x, int y)
        {
            if (tile.Sprite != null)
            {
                tile.Sprite.Draw(g, x, y);
            }
        }

        public virtual ITileBrush DrawOn(ScreenDocument screen, int tile_x, int tile_y)
        {
            var old = screen.TileAt(tile_x, tile_y);
            
            if (old == null)
            {
                return null;
            }

            if (old.Id == tile.Id)
            {
                return null;
            }

            screen.ChangeTile(tile_x, tile_y, tile.Id);
            return new SingleTileBrush(old);
        }

        public override String ToString() {
            return "Tile Id: (" + tile + ")";
        }

        public IEnumerable<TileBrushCell> Cells() { yield return new TileBrushCell(0, 0, tile); }
    }

    public class TileBrush : ITileBrush
    {
        private TileBrushCell[][] cells;
        
        public int Height { get; private set; }
        public int Width { get; private set; }

        public int CellSize { get; private set; }

        public TileBrush(int width, int height)
        {
            Reset(width, height);
        }

        public void Reset(int width, int height)
        {
            TileBrushCell[][] newcells = new TileBrushCell[width][];
            for (int i = 0; i < width; i++)
            {
                newcells[i] = new TileBrushCell[height];
                if (cells != null && i < Width) // old width
                {
                    for (int j = 0; j < height; j++)
                    {
                        if (j < Height) newcells[i][j] = new TileBrushCell(i, j, cells[i][j].tile);
                    }
                }
            }

            cells = newcells;
            Height = height;
            Width = width;
        }

        public void AddTile(Tile tile, int x, int y)
        {
            TileBrushCell cell = new TileBrushCell {x = x, y = y, tile = tile};
            cells[x][y] = cell;
            CellSize = (int)tile.Width;
        }

        #region ITileBrush Members

        /// <summary>
        /// Draws the brush onto the given screen at the given tile location.
        /// Returns an "undo brush" - a brush of all tiles that were overwritten.
        /// Returns null if no tiles were changed.
        /// </summary>
        public ITileBrush DrawOn(ScreenDocument screen, int tile_x, int tile_y)
        {
            TileBrush undo = new TileBrush(Width, Height);
            bool changed = false;
            foreach (TileBrushCell[] col in cells) {
                foreach (TileBrushCell cell in col) {
                    var old = screen.TileAt(cell.x + tile_x, cell.y + tile_y);

                    if (old == null)
                        continue;
                    undo.AddTile(old, cell.x, cell.y);
                    if (old.Id != cell.tile.Id) {
                        changed = true;
                        screen.ChangeTile(cell.x + tile_x, cell.y + tile_y, cell.tile.Id);
                    }
                }
            }

            if (changed) return undo;
            return null;
        }

        public void DrawOn(Graphics g, int x, int y)
        {
            foreach (TileBrushCell cell in Cells())
            {
                if (cell.tile == null) continue;

                if (cell.tile.Sprite != null)
                {
                    cell.tile.Sprite.Draw(g, x + cell.x * cell.tile.Width, y + cell.y * cell.tile.Height);
                }
            }
        }

        public IEnumerable<TileBrushCell> Cells()
        {
            return cells.SelectMany(col => col);
        }

        #endregion
    }
}

