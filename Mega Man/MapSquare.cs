using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace Mega_Man
{
    public class MapSquare
    {
        public MegaMan.Tile Tile { get; protected set; }
        public int X { get; protected set; }
        public int Y { get; protected set; }
        public float ScreenX { get; protected set; }
        public float ScreenY { get; protected set; }

        protected RectangleF basisBox;
        protected RectangleF boundBox;
        protected RectangleF flipBox;

        public RectangleF BoundBox { get { return basisBox; } }
        public RectangleF BlockBox
        {
            get
            {
                if (Game.CurrentGame.GravityFlip)
                {
                    return flipBox;
                }
                return boundBox;
            }
        }

        public MapSquare(MegaMan.Screen screen, MegaMan.Tile tile, int x, int y, float screenX, float screenY)
        {
            Tile = tile;
            X = x;
            Y = y;
            ScreenX = screenX;
            ScreenY = screenY;

            basisBox = Tile.Sprite.BoundBox;
            basisBox.Offset(ScreenX, ScreenY);
            basisBox.Offset(-Tile.Sprite.HotSpot.X, -Tile.Sprite.HotSpot.Y);

            if (this.Tile.Properties.Blocking)
            {
                boundBox = flipBox = basisBox;
            }
            else if (this.Tile.Properties.Climbable)
            {
                MegaMan.Tile below = screen.TileAt(this.X, this.Y + 1);
                if (below != null && !below.Properties.Climbable)
                {
                    flipBox = basisBox;
                    flipBox.Offset(0, flipBox.Height - 4);
                    flipBox.Height = 4;
                }
                else flipBox = RectangleF.Empty;

                MegaMan.Tile above = screen.TileAt(this.X, this.Y - 1);
                if (above != null && !above.Properties.Climbable)
                {
                    boundBox = basisBox;
                    boundBox.Height = 4;
                }
                else boundBox = RectangleF.Empty;
            }
            else
            {
                flipBox = boundBox = RectangleF.Empty;
            }
        }
    }
}
