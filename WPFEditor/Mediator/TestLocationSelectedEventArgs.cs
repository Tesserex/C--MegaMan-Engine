using System;

namespace MegaMan.Editor.Mediator
{
    public class TestLocationSelectedEventArgs : EventArgs
    {
        public string Screen { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}
