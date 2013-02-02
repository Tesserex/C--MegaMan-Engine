using MegaMan.Editor.Bll;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MegaMan.Editor.Controls
{
    public class StageTileControl : StageControl
    {
        public IToolProvider ToolProvider { get; set; }

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
