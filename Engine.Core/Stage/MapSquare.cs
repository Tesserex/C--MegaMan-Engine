using MegaMan.Common;
using MegaMan.Common.Geometry;

namespace MegaMan.Engine
{
    public class MapSquare : IMapSquare
    {
        public Tile Tile { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        private IScreenLayer layer;
        private int screenX;
        private int screenY;
        public int ScreenX { get { return screenX + layer.LocationX; } }
        public int ScreenY { get { return screenY + layer.LocationY; } }

        private readonly Rectangle blockBox;
        private Rectangle ladderBox;
        private Rectangle flipLadderBox;
        private bool ladderBoxesLoaded;

        public Rectangle BoundBox
        {
            get
            {
                var box = blockBox;
                box.Offset(ScreenX, ScreenY);
                return box;
            }
        }

        public Rectangle BlockBox
        {
            get
            {
                Rectangle box;

                if (Tile.Properties.Blocking)
                {
                    box = blockBox;
                }
                else if (Tile.Properties.Climbable)
                {
                    if (!ladderBoxesLoaded)
                    {
                        LoadLadderBoxes();
                    }

                    if (layer.Stage.IsGravityFlipped)
                    {
                        box = flipLadderBox;
                    }
                    else
                    {
                        box = ladderBox;
                    }
                }
                else
                {
                    box = Rectangle.Empty;
                }

                box.Offset(ScreenX, ScreenY);
                return box;
            }
        }

        public MapSquare(IScreenLayer layer, Tile tile, int x, int y, int tilesize)
        {
            this.layer = layer;
            Tile = tile;
            X = x;
            Y = y;
            screenX = x * tilesize;
            screenY = y * tilesize;

            var commonBox = Tile.Sprite.BoundBox;

            blockBox = new Rectangle(commonBox.X, commonBox.Y, commonBox.Width, commonBox.Height);
            blockBox.Offset(-Tile.Sprite.HotSpot.X, -Tile.Sprite.HotSpot.Y);
        }

        private void LoadLadderBoxes()
        {
            var tilesize = Tile.Width;

            var below = layer.SquareAt(ScreenX, ScreenY + tilesize);
            if (below != null && !below.Tile.Properties.Climbable)
            {
                flipLadderBox = blockBox;
                flipLadderBox.Offset(0, flipLadderBox.Height - 4);
                flipLadderBox.Height = 4;
            }
            else flipLadderBox = Rectangle.Empty;

            var above = layer.SquareAt(ScreenX, ScreenY - tilesize);
            if (above != null && !above.Tile.Properties.Climbable)
            {
                ladderBox = blockBox;
                ladderBox.Height = 4;
            }
            else ladderBox = Rectangle.Empty;

            ladderBoxesLoaded = true;
        }

        public TileProperties Properties
        {
            get { return Tile.Properties; }
        }
    }
}
