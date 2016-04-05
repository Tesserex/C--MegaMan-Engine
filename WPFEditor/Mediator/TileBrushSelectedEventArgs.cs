using System;
using MegaMan.Editor.Bll.Tools;

namespace MegaMan.Editor.Mediator
{
    class TileBrushSelectedEventArgs : EventArgs
    {
        public ITileBrush TileBrush { get; set; }
    }
}
