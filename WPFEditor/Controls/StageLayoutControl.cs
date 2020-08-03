using System.Linq;
using MegaMan.Editor.Bll;
using MegaMan.Editor.Bll.Algorithms;
using MegaMan.Editor.Mediator;

namespace MegaMan.Editor.Controls
{
    public class StageLayoutControl : StageControl
    {
        public StageLayoutControl()
        {
            ViewModelMediator.Current.GetEvent<LayoutScreenDroppedEventArgs>().Subscribe(screenDropped);
        }

        protected override ScreenCanvas CreateScreenCanvas(ScreenDocument screen)
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

            var screenData = _screens.Values.Select(canvas => new ScreenWithPosition() {
                Screen = canvas.Screen,
                Bounds = new System.Windows.Rect(canvas.Margin.Left, canvas.Margin.Top, canvas.Screen.PixelWidth, canvas.Screen.PixelHeight)
            });

            var targetScreen = screenData.Single(x => x.Screen == screenCanvas.Screen);

            var joiner = new ScreenSnapJoiner();
            joiner.SnapScreenJoin(targetScreen, screenData);

            UnfreezeLayout();
        }
    }
}
