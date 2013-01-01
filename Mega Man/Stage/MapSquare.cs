using System.Drawing;
using MegaMan.Common;
using Microsoft.Xna.Framework.Graphics;

namespace MegaMan.Engine
{
    public class MapSquare
    {
        public Tile Tile { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        private ScreenLayer layer;
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

                    if (Game.CurrentGame.GravityFlip)
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

        public MapSquare(ScreenLayer layer, Tile tile, int x, int y, int tilesize)
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

            Tile below = layer.SquareAt(ScreenX, ScreenY + tilesize).Tile;
            if (below != null && !below.Properties.Climbable)
            {
                flipLadderBox = blockBox;
                flipLadderBox.Offset(0, flipLadderBox.Height - 4);
                flipLadderBox.Height = 4;
            }
            else flipLadderBox = RectangleF.Empty;

            Tile above = layer.SquareAt(ScreenX, ScreenY - tilesize).Tile;
            if (above != null && !above.Properties.Climbable)
            {
                ladderBox = blockBox;
                ladderBox.Height = 4;
            }
            else ladderBox = RectangleF.Empty;

            ladderBoxesLoaded = true;
        }

        public void Draw(SpriteBatch batch, Microsoft.Xna.Framework.Color color, float posX, float posY)
        {
            if (Tile.Sprite != null)
            {
                (Tile.Sprite.Drawer as XnaSpriteDrawer).DrawXna(batch, color, posX, posY);
            }
        }
    }
}
