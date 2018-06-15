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
        private float screenX;
        private float screenY;
        public float ScreenX { get { return screenX + layer.LocationX; } }
        public float ScreenY { get { return screenY + layer.LocationY; } }

        private readonly RectangleF blockBox;
        private RectangleF ladderBox;
        private RectangleF flipLadderBox;
        private bool ladderBoxesLoaded;

        public RectangleF BoundBox
        {
            get
            {
                var box = blockBox;
                box.Offset(ScreenX, ScreenY);
                return box;
            } 
        }

        public RectangleF BlockBox
        {
            get
            {
                RectangleF box;

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
                    box = RectangleF.Empty;
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

            blockBox = new RectangleF(commonBox.X, commonBox.Y, commonBox.Width, commonBox.Height);
            blockBox.Offset(-Tile.Sprite.HotSpot.X, -Tile.Sprite.HotSpot.Y);
        }

        private void LoadLadderBoxes()
        {
            var tilesize = Tile.Width;

            MapSquare below = layer.SquareAt(ScreenX, ScreenY + tilesize);
            if (below != null && !below.Tile.Properties.Climbable)
            {
                flipLadderBox = blockBox;
                flipLadderBox.Offset(0, flipLadderBox.Height - 4);
                flipLadderBox.Height = 4;
            }
            else flipLadderBox = RectangleF.Empty;

            MapSquare above = layer.SquareAt(ScreenX, ScreenY - tilesize);
            if (above != null && !above.Tile.Properties.Climbable)
            {
                ladderBox = blockBox;
                ladderBox.Height = 4;
            }
            else ladderBox = RectangleF.Empty;

            ladderBoxesLoaded = true;
        }

        public TileProperties Properties
        {
            get { return Tile.Properties; }
        }
    }
}
