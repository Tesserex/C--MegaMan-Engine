using MegaMan.Editor.Bll;
using MegaMan.Editor.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Documents;
using System.Windows.Input;

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

        protected override void DestroyScreenCanvas(ScreenCanvas canvas)
        {
            
        }
    }
}
