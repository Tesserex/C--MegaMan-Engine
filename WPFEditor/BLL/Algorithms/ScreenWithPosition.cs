using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MegaMan.Common;

namespace MegaMan.Editor.Bll.Algorithms
{
    /// <summary>
    /// Describes a ScreenDocument with its visible position on screen,
    /// used for creating joins based on user drag and drop.
    /// </summary>
    public class ScreenWithPosition
    {
        public ScreenDocument Screen { get; set; }
        public Rect Bounds { get; set; }

        public double RightDistanceTo(ScreenWithPosition second)
        {
            if ((Bounds.Top < second.Bounds.Bottom) && (Bounds.Bottom > second.Bounds.Top))
            {
                return Math.Abs(second.Bounds.Left - Bounds.Right);
            }

            return double.PositiveInfinity;
        }

        public double DownDistanceTo(ScreenWithPosition second)
        {
            if ((Bounds.Left < second.Bounds.Right) && (Bounds.Right > second.Bounds.Left))
            {
                return Math.Abs(second.Bounds.Top - Bounds.Bottom);
            }

            return double.PositiveInfinity;
        }

        public void JoinRightwardTo(ScreenWithPosition other)
        {
            var tileTopOne = (int)Math.Round(Bounds.Top / Screen.Tileset.TileSize);
            var tileTopTwo = (int)Math.Round(other.Bounds.Top / Screen.Tileset.TileSize);

            var startPoint = Math.Max(tileTopOne, tileTopTwo);
            var endPoint = Math.Min(tileTopOne + Screen.Height, tileTopTwo + other.Screen.Height);

            var startTileOne = (startPoint - tileTopOne);
            var startTileTwo = (startPoint - tileTopTwo);
            var length = endPoint - startPoint;

            var join = new Join();
            join.ScreenOne = Screen.Name;
            join.ScreenTwo = other.Screen.Name;
            join.Direction = JoinDirection.Both;
            join.Type = JoinType.Vertical;
            join.OffsetOne = startTileOne;
            join.OffsetTwo = startTileTwo;
            join.Size = length;

            Screen.Stage.AddJoin(join);
        }

        public void JoinDownwardTo(ScreenWithPosition other)
        {
            var tileLeftOne = (int)Math.Round(Bounds.Left / Screen.Tileset.TileSize);
            var tileLeftTwo = (int)Math.Round(other.Bounds.Left / Screen.Tileset.TileSize);

            var startPoint = Math.Max(tileLeftOne, tileLeftTwo);
            var endPoint = Math.Min(tileLeftOne + Screen.Width, tileLeftTwo + other.Screen.Width);

            var startTileOne = (startPoint - tileLeftOne);
            var startTileTwo = (startPoint - tileLeftTwo);
            var length = endPoint - startPoint;

            var join = new Join();
            join.ScreenOne = Screen.Name;
            join.ScreenTwo = other.Screen.Name;
            join.Direction = JoinDirection.Both;
            join.Type = JoinType.Horizontal;
            join.OffsetOne = startTileOne;
            join.OffsetTwo = startTileTwo;
            join.Size = length;

            Screen.Stage.AddJoin(join);
        }
    }
}
