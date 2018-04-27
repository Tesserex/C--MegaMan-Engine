using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Controls
{
    public class StageEntitiesControl : StageControl
    {
        protected override ScreenCanvas CreateScreenCanvas(ScreenDocument screen)
        {
            var canvas = new EntitiesScreenCanvas(ToolProvider);
            canvas.Screen = screen;

            return canvas;
        }
    }
}
