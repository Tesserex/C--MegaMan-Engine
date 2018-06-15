using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls
{
    public class StageTileControl : StageControl
    {
        protected override ScreenCanvas CreateScreenCanvas(ScreenDocument screen)
        {
            var canvas = new TileScreenCanvas(ToolProvider);
            canvas.Screen = screen;

            return canvas;
        }
    }
}
