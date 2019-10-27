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
        public void SnapScreenJoin(ScreenWithPosition targetScreen, IEnumerable<ScreenWithPosition> allScreens)
        {
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

                if (rightDistance < 10)
                {
                    targetScreen.JoinRightwardTo(neighbor);
                }
                else if (leftDistance < 10)
                {
                    neighbor.JoinRightwardTo(targetScreen);
                }

                if (downDistance < 10)
                {
                    targetScreen.JoinDownwardTo(neighbor);
                }
                else if (upDistance < 10)
                {
                    neighbor.JoinDownwardTo(targetScreen);
                }
            }
        }
    }
}
