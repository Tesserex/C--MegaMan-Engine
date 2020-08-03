using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Bll.Algorithms
{
    public class ScreenSnapJoiner
    {
        private const int THRESH = 10;

        public void SnapScreenJoin(ScreenWithPosition targetScreen, IEnumerable<ScreenWithPosition> allScreens)
        {
            var halfTile = targetScreen.Screen.TileSize / 2;

            targetScreen.Screen.SeverAllJoins();

            foreach (var neighbor in allScreens)
            {
                if (neighbor.Screen == targetScreen.Screen)
                {
                    continue;
                }

                var rightDistance = targetScreen.RightDistanceTo(neighbor);
                var leftDistance = neighbor.RightDistanceTo(targetScreen);
                var downDistance = targetScreen.DownDistanceTo(neighbor);
                var upDistance = neighbor.DownDistanceTo(targetScreen);

                var horizOverlap = rightDistance > halfTile && leftDistance > halfTile;
                var vertOverlap = downDistance > halfTile && upDistance > halfTile;

                if (rightDistance <= THRESH && vertOverlap)
                {
                    targetScreen.JoinRightwardTo(neighbor);
                }
                else if (leftDistance <= THRESH && vertOverlap)
                {
                    neighbor.JoinRightwardTo(targetScreen);
                }

                if (downDistance <= THRESH && horizOverlap)
                {
                    targetScreen.JoinDownwardTo(neighbor);
                }
                else if (upDistance <= THRESH && horizOverlap)
                {
                    neighbor.JoinDownwardTo(targetScreen);
                }
            }
        }
    }
}
