using System.Drawing;

namespace Mega_Man
{
    public class MapSquare
    {
        public MegaMan.Tile Tile { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public float ScreenX { get; private set; }
        public float ScreenY { get; private set; }

        private readonly RectangleF basisBox;
        private readonly RectangleF boundBox;
        private readonly RectangleF flipBox;

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

            if (Tile.Properties.Blocking)
            {
                boundBox = flipBox = basisBox;
            }
            else if (Tile.Properties.Climbable)
            {
                MegaMan.Tile below = screen.TileAt(X, Y + 1);
                if (below != null && !below.Properties.Climbable)
                {
                    flipBox = basisBox;
                    flipBox.Offset(0, flipBox.Height - 4);
                    flipBox.Height = 4;
                }
                else flipBox = RectangleF.Empty;

                MegaMan.Tile above = screen.TileAt(X, Y - 1);
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
