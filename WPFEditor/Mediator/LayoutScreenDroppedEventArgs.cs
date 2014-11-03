using System;
using MegaMan.Editor.Controls;

namespace MegaMan.Editor.Mediator
{
    public class LayoutScreenDroppedEventArgs : EventArgs
    {
        public ScreenCanvas Canvas { get; set; }
    }
}
