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

        private readonly RectangleF basisBox;
        private readonly RectangleF boundBox;
        private readonly RectangleF flipBox;

        public RectangleF BoundBox
        {
            get
            {
                var box = basisBox;
                box.Offset(ScreenX, ScreenY);
                return box;
            } 
        }

        public RectangleF BlockBox
        {
            get
            {
                var box = boundBox;
                if (Game.CurrentGame.GravityFlip)
                {
                    box = flipBox;
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

            basisBox = Tile.Sprite.BoundBox;
            basisBox.Offset(-Tile.Sprite.HotSpot.X, -Tile.Sprite.HotSpot.Y);

            if (Tile.Properties.Blocking)
            {
                boundBox = flipBox = basisBox;
            }
            else if (Tile.Properties.Climbable)
            {
                Tile below = layer.SquareAt(ScreenX, ScreenY + tilesize).Tile;
                if (below != null && !below.Properties.Climbable)
                {
                    flipBox = basisBox;
                    flipBox.Offset(0, flipBox.Height - 4);
                    flipBox.Height = 4;
                }
                else flipBox = RectangleF.Empty;

                Tile above = layer.SquareAt(ScreenX, ScreenY - tilesize).Tile;
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

        public void Draw(SpriteBatch batch, Microsoft.Xna.Framework.Color color, float posX, float posY)
        {
            if (Tile.Sprite != null)
                Tile.Sprite.DrawXna(batch, color, (int)posX, (int)posY);
        }
    }
}
