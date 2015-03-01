using System;
using MegaMan.Common.Geometry;
using MegaMan.Editor.Bll;

namespace MegaMan.Editor.Mediator
{
    public class SelectionChangedEventArgs : EventArgs
    {
        public ScreenDocument Screen { get; set; }
        public Rectangle? Selection { get; set; }
    }
}
