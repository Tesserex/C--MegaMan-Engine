using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls
{
    public class StageLayoutControl : StageControl
    {
        public StageLayoutControl()
            : base()
        {
            ViewModelMediator.Current.GetEvent<LayoutScreenDroppedEventArgs>().Subscribe(screenDropped);
        }

        protected override ScreenCanvas CreateScreenCanvas(Bll.ScreenDocument screen)
        {
            var canvas = new LayoutScreenCanvas(ToolProvider);
            canvas.Screen = screen;

            return canvas;
        }

        private void screenDropped(object sender, LayoutScreenDroppedEventArgs e)
        {
            var screen = (LayoutScreenCanvas)e.Canvas;

            SnapScreenJoin(screen);
        }

        private void SnapScreenJoin(LayoutScreenCanvas screenCanvas)
        {
            FreezeLayout();

            screenCanvas.Screen.SeverAllJoins();

            foreach (LayoutScreenCanvas neighbor in _screens.Values)
            {
                if (neighbor == screenCanvas)
                {
                    continue;
                }

                var rightDistance = screenCanvas.RightDistanceTo(neighbor);
                var leftDistance = neighbor.RightDistanceTo(screenCanvas);
                var downDistance = screenCanvas.DownDistanceTo(neighbor);
                var upDistance = neighbor.DownDistanceTo(screenCanvas);

                if (rightDistance < 10)
                {
                    screenCanvas.JoinRightwardTo(neighbor);
                }
                else if (leftDistance < 10)
                {
                    neighbor.JoinRightwardTo(screenCanvas);
                }

                if (downDistance < 10)
                {
                    screenCanvas.JoinDownwardTo(neighbor);
                }
                else if (upDistance < 10)
                {
                    neighbor.JoinDownwardTo(screenCanvas);
                }
            }

            UnfreezeLayout();
        }
    }
}
